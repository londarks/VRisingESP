# VRisingESP

Mod ESP (Extra Sensory Perception) para V Rising. Projeto educacional para aprender game hacking com BepInEx + Unity ECS.

## Estrutura do Projeto

```
VRisingESP/
├── Plugin.cs                  # Entry point do BepInEx - inicializa Harmony e componentes
├── API/
│   ├── VWorld.cs              # Acesso ao World/EntityManager do Unity ECS
│   └── VExtensions.cs         # Extension methods para Entity (Read, Has, GetPosition, etc)
├── ESP/
│   ├── Aimbot.cs              # Sistema de mira automatica com predicao de projetil
│   ├── AimController.cs       # Controle de input do aimbot (Hold/Toggle + mover mouse)
│   ├── EntityList.cs          # Queries ECS para cada tipo de entidade
│   ├── ItemsSize.cs           # Calculo de tamanho dos elementos na tela
│   ├── Logic.cs               # Loop principal - processa entidades e faz WorldToScreen
│   ├── Primitives.cs          # Desenho baixo nivel (strings, boxes, circulos, X)
│   └── RenderQueue.cs         # Fila thread-safe de comandos de render
├── Patches/
│   ├── BootstrapPatch.cs      # Hook no WorldBootstrap - inicializa queries ECS
│   ├── BuffSystemPatch.cs     # Auto-fishing - detecta buff de pesca
│   ├── ContainerMenuPatch.cs  # Auto-loot - clica "Take All" em containers
│   ├── CurseDebuffPatch.cs    # No Fog - remove nevoa do mapa
│   ├── HUDMenuPatch.cs        # Detecta quando menu do jogo abre/fecha
│   ├── OptionsPanelPatch.cs   # Hook no painel de opcoes
│   ├── ProjectileSystemPatch.cs # Captura velocidade do projetil para predicao
│   └── StartGamePatch.cs      # Pula video de intro
├── UI/
│   ├── Menu.cs                # Menu IMGUI com todas as opcoes (INSERT para abrir)
│   ├── MenuTheme.cs           # Tema dark para o menu
│   └── Overlay.cs             # Renderiza ESP no OnGUI + chama Logic.ProcessAllEntities
├── Utils/
│   ├── Colors.cs              # Paleta de 21 cores para ESP
│   ├── Config.cs              # Sistema de configuracao (BepInEx ConfigFile)
│   ├── MouseSimulator.cs      # Simula cliques/movimento do mouse (Win32 API)
│   ├── OptionsGUI/Components/ # Componentes de UI reutilizaveis
│   │   ├── Dropdown.cs
│   │   ├── Header.cs
│   │   ├── Slider.cs
│   │   └── Toggle.cs
│   └── Prefabs/
│       ├── Items.cs           # Limpeza de nomes de itens (regex)
│       └── VBloods.cs         # Mapeamento PrefabGUID -> nome dos V Bloods
├── VRisingESP.csproj          # Configuracao do build
└── nuget.config               # NuGet feeds (BepInEx + Samboy)
```

## Features

### ESP (Extra Sensory Perception)
- **Players** - nome, nivel de equipamento, HP, tipo de sangue
- **VBlood Carriers** - bosses com nome e HP
- **Blood Sources** - mobs com tipo/qualidade de sangue (filtro por %)
- **Gate Bosses** - bosses de Rift Incursions
- **Items** - itens no chao com nome e quantidade
- **Containers** - baus e containers
- **Ores** - minerios
- **Plants** - plantas coletaveis
- **Fishing Spots** - pontos de pesca
- **Horses** - cavalos com stats (velocidade, aceleracao, rotacao)
- **Servants** - servos com nivel e expertise
- **Carriages** - carruagens

### Aimbot
- Mira automatica com **predicao de projetil** (equacao de interceptacao)
- Prioriza alvos por: distancia, proximidade do cursor, HP baixo, tipo
- Modos: **Hold** (segura tecla) ou **Toggle** (aperta pra ligar/desligar)
- Configuravel: distancia maxima, pesos, cooldown de troca de alvo

### Extras
- **Auto-Fishing** - detecta buff de pesca e clica automaticamente
- **Auto-Loot** - aperta "Take All" quando container abre
- **No Fog** - remove nevoa do mapa/minimapa

## Teclas

| Tecla | Funcao |
|-------|--------|
| INSERT | Abrir/fechar menu |
| Configuravel | Aimbot (padrao: Mouse4) |

## Como Compilar

```bash
cd VRisingESP
dotnet build
```

A DLL sera gerada em `bin/Debug/net6.0/VRisingESP.dll`.

## Como Instalar

1. Instale o BepInEx 6 IL2CPP no V Rising (Thunderstore: BepInExPack_V_Rising)
2. Copie `VRisingESP.dll` para `VRising/BepInEx/plugins/`
3. Inicie o jogo

## Como Funciona (Arquitetura)

```
Jogo inicia
  → BepInEx carrega Plugin.cs
    → Harmony aplica patches em sistemas do jogo
    → AddComponent<Menu>() e AddComponent<Overlay>()

WorldBootstrapPatch detecta mundo inicializado
  → EntityList.InitializeQueries() cria EntityQuery para cada tipo

A cada frame (Overlay.Update):
  → Logic.ProcessAllEntities()
    → Para cada tipo de entidade habilitado:
      → EntityQuery retorna entidades matching
      → Para cada entidade: posicao → WorldToScreen → RenderQueue
    → Aimbot.UpdateAimData() calcula melhor alvo

OnGUI (Overlay.OnGUI):
  → RenderQueue.DrawQueued() desenha tudo na tela
  → Menu desenha interface se INSERT pressionado
```

## Dependencias

- **BepInEx 6.0.0-be.733** (IL2CPP) - framework de modding
- **Harmony 2.x** - patching de metodos em runtime
- **VRising.Unhollowed.Client 1.1.8** - bindings IL2CPP do V Rising

## Configuracao

As configs ficam em `VRising/BepInEx/config/VRisingESP.cfg` (gerado automaticamente).
Edite o arquivo ou use o menu in-game (INSERT).


```
 copy VRisingESP\bin\Debug\net6.0\VRisingESP.dll "C:\Program Files (x86)\Steam\steamapps\common\VRising\BepInEx\plugins\"
 
 
 ```