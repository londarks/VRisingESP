# BABEL - V Rising ESP

Mod para V Rising com ESP, Aimbot, Auto-Parry, Smart Assist, Radar e mais.

---

## Instalacao

### 1. Baixar BepInEx

**[Download BepInEx 6.0.0-be.733](https://builds.bepinex.dev/projects/bepinex_be/733/BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.733%2B995f049.zip)**

### 2. Instalar BepInEx

1. Localize a pasta do V Rising:
   ```
   C:\Program Files (x86)\Steam\steamapps\common\VRising\
   ```
   *(Steam > V Rising > clique direito > Gerenciar > Navegar ate os arquivos locais)*

2. Extraia **todo o conteudo** do zip dentro dessa pasta (onde fica o `VRising.exe`)

3. Abra o jogo **uma vez** e feche — BepInEx cria as pastas automaticamente

4. Confirme que existe a pasta `BepInEx/plugins/`

### 3. Instalar o Mod

Copie `VRisingESP.dll` para:
```
VRising/BepInEx/plugins/VRisingESP.dll
```

### 4. Jogar

1. Abra o jogo normalmente
2. Pressione **Insert** para abrir o menu
3. Use **Shift + Click Esquerdo** em um inimigo para travar o alvo

---

## Funcionalidades

| Feature | Descricao |
|---------|-----------|
| **ESP** | Jogadores, mobs, VBlood, itens, recursos na tela. Modos: Caixas, Cantos, HP Bar, Ponto |
| **Aimbot** | Mira automatica com predicao de projetil. Lock Target com Shift+Click |
| **Auto-Parry** | Parry automatico contra spells e projeteis inimigos |
| **Smart Assist** | Ativa aimbot durante casts + quick-cast ao trocar arma |
| **Radar** | Mini-mapa com entidades proximas |
| **Auto-Fish** | Pesca automatica |
| **Auto-Loot** | Loot automatico |
| **No Fog** | Remove neblina do mapa |

## Teclas

| Tecla | Acao |
|-------|------|
| `Insert` | Abrir/fechar menu |
| `Shift + Click Esquerdo` | Lock/unlock alvo |
| `Left Shift` (segurar) | Ativar aimbot (modo manual) |

---

## Como Compilar

```bash
cd VRisingESP
dotnet build
```

DLL gerada em `bin/Debug/net6.0/VRisingESP.dll`.

## Requisitos

- V Rising (Steam)
- Windows 10/11 64-bit
- BepInEx 6.0.0-be.733 IL2CPP