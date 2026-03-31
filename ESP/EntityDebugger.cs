using System;
using System.Collections.Generic;
using ExtrasensoryPerception.API;
using ExtrasensoryPerception.Utils;
using ProjectM;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

/// <summary>
/// Debug continuo: monitora mobs perto e loga toda mudanca de estado de combate.
/// F8 = liga/desliga. Fica ativo ate desligar.
/// Loga quando:
/// - Mob comeca a castar (IsCasting muda pra true)
/// - Mob termina de castar
/// - CastAbility muda (nova spell)
/// - Buff novo aparece/desaparece
/// - Mob se move (velocity muda)
/// </summary>
public static class EntityDebugger
{
    private static bool _enabled;
    private static float _lastLogTime;

    // Cache do estado anterior de cada mob pra detectar mudancas
    private struct MobState
    {
        public bool WasCasting;
        public int CastAbilityHash;
        public int BuffCount;
        public bool WasMoving;
    }

    private static readonly Dictionary<int, MobState> _previousStates = new();

    internal static void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            _enabled = !_enabled;
            _previousStates.Clear();
            if (_enabled)
            {
                FileLogger.Start();
                Plugin.Logger.LogInfo($"[Debug] ATIVADO - salvando em {FileLogger.FilePath}. F8 pra desligar.");
            }
            else
            {
                FileLogger.Stop();
                Plugin.Logger.LogInfo("[Debug] DESATIVADO - log salvo.");
            }
        }

        // F9 = analisar MEU personagem (spells, cooldowns, buffs)
        if (Input.GetKeyDown(KeyCode.F9))
        {
            AnalyzeLocalPlayer();
        }
    }

    /// <summary>
    /// Analisa o personagem local — spells equipadas, cooldowns, buffs.
    /// </summary>
    private static void AnalyzeLocalPlayer()
    {
        var localChar = EntityList.LocalCharacter;
        if (localChar == Entity.Null || !localChar.Exists())
        {
            Plugin.Logger.LogInfo("[Debug] LocalCharacter nao encontrado");
            return;
        }

        try
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("\n========== MEU PERSONAGEM ==========");

            // Health
            if (localChar.TryGetComponent<Health>(out var health))
                sb.AppendLine($"HP: {health.Value:F0}/{health.MaxHealth.Value:F0}");

            // CollisionRadius
            if (localChar.TryGetComponent<CollisionRadius>(out var myRadius))
                sb.AppendLine($"CollisionRadius: {myRadius.Radius:F3}");
            else
                sb.AppendLine("CollisionRadius: NAO TEM");

            // AbilityBar_Shared
            if (localChar.TryGetComponent<AbilityBar_Shared>(out var bar))
            {
                sb.AppendLine($"IsCasting: {bar.SyncedIsCasting}");
                sb.AppendLine($"GlobalCD: {bar.GlobalCooldown:F2}");
            }

            // AbilityGroupSlotBuffer - slots de habilidade
            if (VWorld.EntityManager.HasBuffer<AbilityGroupSlotBuffer>(localChar))
            {
                var slots = VWorld.EntityManager.GetBuffer<AbilityGroupSlotBuffer>(localChar);
                sb.AppendLine($"\n--- SLOTS ({slots.Length}) ---");
                var map = VWorld.PrefabLookupMap;
                double serverTime = VWorld.Game.Time.ElapsedTime;

                for (int i = 0; i < slots.Length; i++)
                {
                    var slot = slots[i];
                    string name = map.GetName(slot.BaseAbilityGroupOnSlot);
                    var slotEntity = slot.GroupSlotEntity._Entity;

                    string cdInfo = "sem entity";
                    if (slotEntity != Entity.Null && slotEntity.Exists())
                    {
                        if (slotEntity.TryGetComponent<AbilityGroupSlot>(out var groupSlot))
                        {
                            cdInfo = $"SlotId={groupSlot.SlotId}";

                            // Tentar cooldown no StateEntity
                            var stateEntity = groupSlot.StateEntity._Entity;
                            if (stateEntity != Entity.Null && stateEntity.Exists())
                            {
                                cdInfo += $" StateEntity={stateEntity}";
                                if (stateEntity.TryGetComponent<AbilityCooldownState>(out var cdState))
                                {
                                    float remaining = (float)System.Math.Max(0, cdState.CooldownEndTime - serverTime);
                                    cdInfo += $" CD={remaining:F1}s/{cdState.CurrentCooldown:F1}s";
                                }
                                else
                                {
                                    cdInfo += " (sem AbilityCooldownState)";
                                }

                                // Listar componentes do StateEntity
                                var stateComps = stateEntity.GetAllComponents();
                                cdInfo += " Comps=[";
                                for (int j = 0; j < System.Math.Min(stateComps.Length, 15); j++)
                                {
                                    if (j > 0) cdInfo += ",";
                                    cdInfo += stateComps[j].GetManagedType()?.Name ?? "?";
                                }
                                cdInfo += "]";
                                stateComps.Dispose();
                            }
                            else
                            {
                                cdInfo += " StateEntity=NULL";
                            }

                            // Tentar cooldown no proprio slotEntity
                            if (slotEntity.TryGetComponent<AbilityCooldownState>(out var cdState2))
                            {
                                float remaining2 = (float)System.Math.Max(0, cdState2.CooldownEndTime - serverTime);
                                cdInfo += $" SlotCD={remaining2:F1}s/{cdState2.CurrentCooldown:F1}s";
                            }
                        }
                    }

                    sb.AppendLine($"  Slot[{i}]: {name} | {cdInfo}");
                }
            }
            else
            {
                sb.AppendLine("AbilityGroupSlotBuffer: NAO TEM");
            }

            // BuffBuffer
            if (VWorld.EntityManager.HasBuffer<BuffBuffer>(localChar))
            {
                var buffs = VWorld.EntityManager.GetBuffer<BuffBuffer>(localChar);
                sb.AppendLine($"\n--- BUFFS ({buffs.Length}) ---");
                var map = VWorld.PrefabLookupMap;
                for (int i = 0; i < System.Math.Min(buffs.Length, 15); i++)
                {
                    sb.AppendLine($"  [{i}] {map.GetName(buffs[i].PrefabGuid)}");
                }
            }

            sb.AppendLine("====================================\n");
            Plugin.Logger.LogInfo(sb.ToString());
        }
        catch (System.Exception ex)
        {
            Plugin.Logger.LogError($"[Debug] Erro ao analisar player: {ex.Message}");
        }
    }

    /// <summary>
    /// Chamado pra cada mob/player processado. Monitora mudancas de estado.
    /// </summary>
    internal static void AnalyzeNearbyEntity(Entity entity, string entityName, float distance)
    {
        if (!_enabled) return;
        if (distance > 15f) return;

        try
        {
            int entityId = entity.Index;

            // Ler estado atual
            bool isCasting = false;
            int castAbilityHash = 0;
            string castAbilityName = "";
            string castGroupName = "";
            float castTime = 0;
            float globalCd = 0;

            if (entity.TryGetComponent<AbilityBar_Shared>(out var abilityBar))
            {
                isCasting = abilityBar.SyncedIsCasting;
                castAbilityHash = abilityBar.CastAbilityPrefabGuid._Value;
                castTime = abilityBar.CastTime;
                globalCd = abilityBar.GlobalCooldown;

                if (abilityBar.CastAbilityPrefabGuid != PrefabGUID.Empty)
                {
                    var map = VWorld.PrefabLookupMap;
                    castAbilityName = map.GetName(abilityBar.CastAbilityPrefabGuid);
                    castGroupName = map.GetName(abilityBar.CastGroupPrefabGuid);
                }
            }

            int buffCount = 0;
            string newBuffs = "";
            if (VWorld.EntityManager.HasBuffer<BuffBuffer>(entity))
            {
                var buffs = VWorld.EntityManager.GetBuffer<BuffBuffer>(entity);
                buffCount = buffs.Length;

                // Listar buffs se mudou
                var map2 = VWorld.PrefabLookupMap;
                for (int i = 0; i < Math.Min(buffs.Length, 5); i++)
                {
                    if (newBuffs.Length > 0) newBuffs += ", ";
                    newBuffs += map2.GetName(buffs[i].PrefabGuid);
                }
            }

            bool isMoving = false;
            if (entity.Has<Velocity>())
            {
                var vel = entity.Read<Velocity>();
                isMoving = vel.Value.x * vel.Value.x + vel.Value.z * vel.Value.z > 0.5f;
            }

            // Comparar com estado anterior
            if (!_previousStates.TryGetValue(entityId, out var prev))
            {
                prev = new MobState();
            }

            bool changed = false;
            string changes = "";

            // CollisionRadius do mob
            float mobCollisionRadius = 0f;
            if (entity.TryGetComponent<CollisionRadius>(out var collRadius))
            {
                mobCollisionRadius = collRadius.Radius;
            }

            // Direção e mira do mob
            string aimInfo = "";
            bool aimingAtPlayer = false;
            if (isCasting)
            {
                var localChar = EntityList.LocalCharacter;
                if (localChar != Entity.Null && localChar.Exists())
                {
                    Vector3 mobPos = entity.GetPosition();
                    Vector3 playerPos = localChar.GetPosition();
                    Vector3 mobToPlayer = (playerPos - mobPos).normalized;

                    // TargetDirection
                    if (entity.TryGetComponent<TargetDirection>(out var targetDir))
                    {
                        Vector3 aimDir3 = new Vector3(targetDir.AimDirection.x, 0, targetDir.AimDirection.z).normalized;
                        float dotTd = Vector3.Dot(aimDir3, mobToPlayer);
                        aimInfo += $" AimDir=({targetDir.AimDirection.x:F2},{targetDir.AimDirection.z:F2}) dot={dotTd:F2}";
                        if (dotTd > 0.5f) aimingAtPlayer = true;
                    }

                    // EntityAimData
                    if (entity.TryGetComponent<EntityAimData>(out var aimData))
                    {
                        // Distancia do AimPosition ao player
                        Vector3 aimPos = new Vector3(aimData.AimPosition.x, 0, aimData.AimPosition.z);
                        Vector3 playerFlat = new Vector3(playerPos.x, 0, playerPos.z);
                        float aimToPlayerDist = Vector3.Distance(aimPos, playerFlat);
                        aimInfo += $" AimPos=({aimData.AimPosition.x:F1},{aimData.AimPosition.z:F1}) distAimToPlayer={aimToPlayerDist:F1}m";
                        aimInfo += $" ProjAim=({aimData.ProjectileAimPosition.x:F1},{aimData.ProjectileAimPosition.z:F1})";

                        // Se o AimPosition esta perto do player, esta mirando em nos
                        if (aimToPlayerDist < 3f) aimingAtPlayer = true;
                    }

                    // Rotation
                    if (entity.TryGetComponent<Rotation>(out var rotation))
                    {
                        // Forward do quaternion
                        var q = rotation.Value;
                        Vector3 forward = new Vector3(
                            2f * (q.value.x * q.value.z + q.value.w * q.value.y),
                            0,
                            1f - 2f * (q.value.x * q.value.x + q.value.y * q.value.y)
                        ).normalized;
                        float dotRot = Vector3.Dot(forward, mobToPlayer);
                        aimInfo += $" RotFwd=({forward.x:F2},{forward.z:F2}) rotDot={dotRot:F2}";
                    }

                    // CollisionRadius do player
                    float playerRadius = 0f;
                    if (localChar.TryGetComponent<CollisionRadius>(out var playerCollRadius))
                        playerRadius = playerCollRadius.Radius;

                    // Calcular se o ataque vai atingir baseado na geometria
                    // Distancia do player à linha de mira do mob
                    if (entity.TryGetComponent<TargetDirection>(out var td2))
                    {
                        Vector3 aimDir2 = new Vector3(td2.AimDirection.x, 0, td2.AimDirection.z).normalized;
                        // Ponto mais proximo na linha de mira ao player
                        Vector3 toPlayer2 = playerPos - mobPos;
                        float projLen = Vector3.Dot(toPlayer2, aimDir2);
                        Vector3 closestPoint = mobPos + aimDir2 * projLen;
                        float distToLine = Vector3.Distance(playerPos, closestPoint);
                        aimInfo += $" PlayerRadius={playerRadius:F2} MobRadius={mobCollisionRadius:F2} DistToAimLine={distToLine:F2}m";

                        // Se distancia do player à linha de mira < soma dos raios, vai atingir
                        float hitThreshold = playerRadius + mobCollisionRadius + 0.5f;
                        bool willHit = distToLine < hitThreshold && projLen > 0;
                        aimInfo += willHit ? " >>> VAI ATINGIR!" : " >>> VAI ERRAR";
                    }

                    aimInfo += aimingAtPlayer ? " MIRANDO" : " NAO_MIRA";
                }
            }

            // Casting mudou
            if (isCasting != prev.WasCasting)
            {
                changed = true;
                if (isCasting)
                    changes += $" >>> COMECOU A CASTAR: {castAbilityName} (grupo: {castGroupName}) CastTime={castTime:F2}s{aimInfo}";
                else
                    changes += $" >>> PAROU DE CASTAR";
            }

            // Spell mudou
            if (castAbilityHash != prev.CastAbilityHash && castAbilityHash != 0)
            {
                changed = true;
                changes += $" >>> NOVA SPELL: {castAbilityName} (PrefabGUID={castAbilityHash})";
            }

            // Buffs mudaram
            if (buffCount != prev.BuffCount)
            {
                changed = true;
                changes += $" >>> BUFFS: {prev.BuffCount}->{buffCount} [{newBuffs}]";
            }

            // Movimento mudou
            if (isMoving != prev.WasMoving)
            {
                changed = true;
                changes += isMoving ? " >>> MOVENDO" : " >>> PAROU";
            }

            // Logar se mudou algo
            if (changed)
            {
                float hp = 0;
                if (entity.TryGetComponent<Health>(out var health))
                    hp = health.Value;

                string logLine = $"[Debug] [{entityName}] dist={distance:F1}m HP={hp:F0} Casting={isCasting} GCD={globalCd:F2}{changes}";
                Plugin.Logger.LogInfo(logLine);
                FileLogger.Log(logLine);
            }

            // Salvar estado
            _previousStates[entityId] = new MobState
            {
                WasCasting = isCasting,
                CastAbilityHash = castAbilityHash,
                BuffCount = buffCount,
                WasMoving = isMoving
            };
        }
        catch (Exception)
        {
            // Silenciar
        }
    }
}
