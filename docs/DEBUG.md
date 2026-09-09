# Relatório de debug — Rock Paper Pistol

Documento para localizar causa de bugs no playtest. Comece pela **sintoma → arquivo**, depois use o catálogo completo.

Cena de teste: `Assets/_Game/Scenes/SampleScene.unity`  
Play cria em runtime o objeto `RockPaperPistol` (`GameSessionDriver` + `AudioManager` + `BattleBoard`).

---

## 1. Fluxo de uma jogada

```
SampleScene
  → BattleBoard.Bootstrap (AfterSceneLoad)
      → GameSessionDriver.SelectDeck(0)
          → RunSession + DeckRuntime (mão 8 básicas + pistola)
      → BattleBoard desenha leque / mesa / HUD
      → clique (GameInput) em CardView
          → GameSessionDriver.PlayFromHandAnimated
              → espera BattleTiming (Selected)
              → RunSession.PlayFromHand
                  → EnemyCardPicker escolhe a carta do oponente
                  → CardComparer resolve naipe / valor / pistola
                  → Encounter atualiza placar
              → etapas Revealed → SuitChecked → ValueChecked → ResultShown
              → GameplayEventBus → AudioManager
              → BattleBoard.RefreshIfNeeded redesenha
```

Onde olhar o estado em Play:

| O quê | Onde |
|---|---|
| Fase da run (`AwaitingDeck`, `InEncounter`, `GameOver`, `Victory`) | `GameSessionDriver` → `Session.Phase` |
| Etapa da animação/resolução | `GameSessionDriver.ResolutionStep` |
| Mão / descarte / carta de fora | `Session.Deck` e `Session.EnemyDeck` |
| Placar e turno | `Session.CurrentEncounter` |
| Último evento de áudio | `AudioManager.LastEvent` |
| Tempos entre etapas | `Assets/_Game/Content/BattleTiming.asset` |

---

## 2. Sintoma → arquivo

| Sintoma | Arquivos | O que checar |
|---|---|---|
| Cartas não aparecem | `BattleBoard.cs`, `CardView.cs`, `PlaceholderArt.cs` | `Awake`/`BuildStage` estourando; `HideBlockingUi` não desligou o `DialogSystem`; material URP |
| Clique / Esc / Pause não respondem | `GameInput.cs`, `BattleBoard.cs`, `DebugPlayView.cs` | Projeto em Input System only; não usar `UnityEngine.Input` |
| Textos ilegíveis ou “REVOLVER” | `CardView.cs`, `BattleBoard.cs` | Labels TMP; pistola do jogador deve dizer **Pistola** |
| Fonte `Arial.ttf` no console | (já removido do HUD) | HUD usa TMP; não voltar `Resources.GetBuiltinResource("Arial.ttf")` |
| Resolução “pula” etapas ou está lenta | `GameSessionDriver.cs`, `BattleTiming.cs` + `.asset` | Delays do asset; `IsResolving` travado se a coroutine parar |
| Comparação errada (naipe/valor/pistola) | `CardComparer.cs`, `Card.cs` | Não reescrever o comparer; ver testes em `PistolComparerTests.cs` |
| IA joga a carta errada | `EnemyCardPicker.cs` | 35% naipe preferido; Múmia trava pistola até o turno 3; último turno força pistola |
| Mão com tamanho errado | `DeckRuntime.cs`, `RunSession.cs`, `DefaultCatalog.cs` | Sempre 8 básicas + 1 pistola; resto em `Excluded` |
| Inimigo / pistola errados | `EnemyDefinition` + assets em `Content/Enemies` | Campo `Pistol` e `PistolAvailableFromTurn` |
| Sem som | `AudioManager.cs`, `GameplayEvent.cs` | Eventos `EnemyCardSelected` / `CardDiscarded` / `TurnEnd` ainda não têm clip próprio de propósito |
| Diálogo tapa o leque | `DialogSystem.prefab`, `DialogTest.cs`, `BattleBoard.HideBlockingUi` | O playtest desliga o GO `DialogSystem` |
| Conteúdo do Inspector “não pega” | `GameContent.asset`, bootstrap runtime | Objeto `RockPaperPistol` não está na cena; dados vêm dos assets / `DefaultCatalog` |

---

## 3. Catálogo de arquivos

### 3.1 Cena, prefab e conteúdo

| Caminho | Função no debug |
|---|---|
| `Assets/_Game/Scenes/SampleScene.unity` | Cena de playtest. Câmera ortográfica, Light 2D, Canvas + EventSystem (Input System) e instância do `DialogSystem`. A mesa **não** está na cena: o `BattleBoard` cria tudo no Play. |
| `Assets/_Game/Prefabs/DialogSystem.prefab` | UI de diálogo (fundo branco nos 20% de baixo). Se ativo, cobre o leque. |
| `Assets/_Game/Content/GameContent.asset` | Catálogo da run: baralho + 3 inimigos. `GameSessionDriver` carrega este asset no Editor. |
| `Assets/_Game/Content/BattleTiming.asset` | Tempos de cada etapa da resolução. Primeiro lugar para afinar animações. |
| `Assets/_Game/Content/Decks/Equilibrado.asset` | Baralho base (Pedra/Papel/Tesoura 1–3) referenciado pelo `GameContent`. |
| `Assets/_Game/Content/Decks/Agressivo.asset` | Baralho extra. Não é o da run atual se o `GameContent` só aponta o Equilibrado. |
| `Assets/_Game/Content/Decks/Contrario.asset` | Idem: asset legado/alternativo. |
| `Assets/_Game/Content/Enemies/EstatuaDePedra.asset` | Inimigo 1. `Pistol = Estátua` (2), naipe preferido Pedra, pistola desde o turno 1. |
| `Assets/_Game/Content/Enemies/Mumia.asset` | Inimigo 2. `Pistol = Múmia` (3), naipe Papel, pistola só a partir do turno **3**. |
| `Assets/_Game/Content/Enemies/Pirata.asset` | Inimigo 3. `Pistol = Pirata` (4), naipe Tesoura, pistola desde o turno 1. |
| `Assets/_Game/Content/Dialog/Dialog 1.asset` | Texto de teste do diálogo (`DialogTest`). Não faz parte da batalha. |
| `Assets/Content/Decks/*` e `Assets/Content/Enemies/*` | Cópias antigas fora de `_Game`. **Não** são as que o playtest carrega. Se o Inspector “não muda nada”, você pode estar editando estes. |
| `Assets/Settings/Renderer2D.asset` | Renderer URP 2D (material Lit por padrão). Sprites de runtime precisam de material 2D senão somem. |
| `Assets/Settings/UniversalRP.asset` | Pipeline URP do projeto. |

### 3.2 Núcleo da mecânica (`Core`) — sem Unity

Use estes arquivos quando a regra do jogo estiver errada. Dá para validar com `dotnet test` / testes EditMode, sem dar Play.

| Caminho | Função no debug |
|---|---|
| `Assets/_Game/Scripts/Core/Card.cs` | Carta: naipe, valor, `CardKind`, `PistolId`. `CreatePistol` define naipe/valor base de cada pistola. `ToString` / nomes para HUD de debug. |
| `Assets/_Game/Scripts/Core/Suit.cs` | Enum Pedra / Papel / Tesoura. |
| `Assets/_Game/Scripts/Core/CardComparer.cs` | **Fonte da verdade da comparação.** Ordem: naipe do Pistoleiro → valor (pistolas inclusas) → desempate de naipe. Não reescrever sem os testes de pistola. |
| `Assets/_Game/Scripts/Core/RoundResolution.cs` | Resultado de um turno (naipes efetivos, valores ajustados, outcome, stake). |
| `Assets/_Game/Scripts/Core/DeckRuntime.cs` | Pilha, mão, descarte, `Excluded`. `PrepareMatchHand`: 8 básicas aleatórias + pistola. |
| `Assets/_Game/Scripts/Core/Encounter.cs` | Um oponente: 7 turnos, placar, stake. Status vitória/derrota. |
| `Assets/_Game/Scripts/Core/RunSession.cs` | Run inteira: 3 inimigos, fases, `SelectDeck`, `PlayFromHand`. Junta decks + encounter + picker. |
| `Assets/_Game/Scripts/Core/EnemyCardPicker.cs` | IA: 35% no naipe preferido (pistola conta), 65% em qualquer jogável; último turno força pistola; Múmia trava antes do turno 3. |
| `Assets/_Game/Scripts/Core/EnemyBehavior.cs` | Rótulo `Defensive` / `Aggressive`. **Não escolhe carta** — a escolha está no `EnemyCardPicker`. |
| `Assets/_Game/Scripts/Core/DefaultCatalog.cs` | Fallback se `GameContent` falhar: baralho base, pistola do Pistoleiro, 3 inimigos. |
| `Assets/_Game/Scripts/Core/GameplayEvent.cs` | Eventos da resolução + `GameplayEventBus`. Áudio e UI só escutam; o Core não toca som. |
| `Assets/_Game/Scripts/Core/RockPaperPistol.Core.asmdef` | Assembly do núcleo (testável sem Unity). |

### 3.3 Dados / ScriptableObjects (`Data`)

| Caminho | Função no debug |
|---|---|
| `Assets/_Game/Scripts/Data/CardData.cs` | Naipe + valor serializados. **Não** carrega pistola — pistola vai no `EnemyDefinition.Pistol`. |
| `Assets/_Game/Scripts/Data/DeckDefinition.cs` | SO de baralho (`Create → Rock Paper Pistol/Baralho`). |
| `Assets/_Game/Scripts/Data/EnemyDefinition.cs` | SO de inimigo: comportamento, naipe preferido, `PistolId`, turno em que a pistola libera. |
| `Assets/_Game/Scripts/Data/GameContent.cs` | SO raiz que lista decks e inimigos da run. |
| `Assets/_Game/Scripts/Data/DialogData.cs` | SO de sequência de diálogo. |
| `Assets/_Game/Scripts/Data/DialogPart.cs` | Uma fala: texto, delay por caractere, se limpa a caixa. |
| `Assets/_Game/Scripts/Data/InterruptionType.cs` | Enum de interrupção de diálogo. Ainda não entra na batalha. |
| `Assets/_Game/Scripts/Data/RockPaperPistol.Data.asmdef` | Assembly dos SOs. |

### 3.4 Playtest Unity (`Unity` + batalha)

| Caminho | Função no debug |
|---|---|
| `Assets/_Game/Scripts/Unity/GameSessionDriver.cs` | Ponte Unity ↔ Core. Sobe a run, anima a resolução por etapas, dispara eventos. Se a jogada “não resolve”, a coroutine `ResolveRound` parou ou `IsResolving` ficou true. |
| `Assets/_Game/Scripts/Unity/BattleTiming.cs` | Script do SO de tempos. Campos que a equipe edita no `BattleTiming.asset`. |
| `Assets/_Game/Scripts/Unity/GameInput.cs` | Único lugar de input do playtest (Input System): clique, Esc, scroll, botão IMGUI. |
| `Assets/_Game/Scripts/Unity/AudioManager.cs` | Ouve o `GameplayEventBus`. Sem clip no Inspector, gera beep. `LastEvent` confirma se o evento chegou. |
| `Assets/_Game/Scripts/Unity/DebugPlayView.cs` | UI IMGUI antiga da mecânica. **Desliga sozinha** se existir `BattleBoard`. Só reaparece se o bootstrap da mesa falhar. |
| `Assets/_Game/Scripts/Unity/Battle/BattleBoard.cs` | Mesa 2D: bootstrap, layout, hover/clique, pause, esconde diálogo. Primeiro arquivo se o Game View estiver errado. |
| `Assets/_Game/Scripts/Unity/Battle/CardView.cs` | Uma carta: frente/verso, hover dourado, collider, labels TMP (naipe/pistola + valor). |
| `Assets/_Game/Scripts/Unity/Battle/PlaceholderArt.cs` | Sprites e beeps gerados em runtime. Material URP 2D (`ApplyVisibleMaterial`). Se o ícone da carta estiver errado, é aqui — não no TMP. |
| `Assets/_Game/Scripts/Unity/RockPaperPistol.Unity.asmdef` | Assembly Unity: Core, Data, Input System, TMP. |

### 3.5 Diálogo

| Caminho | Função no debug |
|---|---|
| `Assets/_Game/Scripts/DialogSystem/DialogSystem.cs` | Escreve a fala na UI (typewriter). |
| `Assets/_Game/Scripts/DialogSystem/DialogTest.cs` | **Liga o diálogo sozinho no Start.** É o que cobria as cartas antes do `HideBlockingUi`. |
| `Assets/_Game/Scripts/DialogSystem/RockPaperPistol.DialogSystem.asmdef` | Assembly do diálogo. |

### 3.6 Editor, utils e testes

| Caminho | Função no debug |
|---|---|
| `Assets/_Game/Scripts/Editor/DefaultContentGenerator.cs` | Menu `Rock Paper Pistol/Gerar conteúdo padrão`. Recria/atualiza `GameContent` e inimigos. |
| `Assets/_Game/Scripts/Utils/Utils.cs` | Load de cena sync/async. Fora do fluxo da batalha. |
| `Assets/Settings/Tests/EditMode/CardComparerTests.cs` | Comparação básica (naipe → valor). |
| `Assets/Settings/Tests/EditMode/PistolComparerTests.cs` | Regras das 4 pistolas. Rode isto se o resultado do turno estiver “absurdo”. |
| `Assets/Settings/Tests/EditMode/DeckRuntimeTests.cs` | Saque 8+1 e `Excluded`. |
| `Assets/Settings/Tests/EditMode/EncounterTests.cs` | Run, 7 turnos, mãos iguais no começo. |
| `Assets/Settings/Tests/EditMode/DefaultCatalogTests.cs` | Nomes, pistolas e ordem dos 3 inimigos. |
| `Assets/Settings/Tests/EditMode/EnemyCardPickerTests.cs` | IA: pool do naipe, lock da Múmia, pistola no último turno. |

---

## 4. Checklist rápido no Play

1. Console limpo? Qualquer exception no `Awake` do `BattleBoard` impede a mesa de nascer.
2. Hierarchy: existe `RockPaperPistol` com `GameSessionDriver`, `AudioManager`, `BattleBoard`?
3. `DialogSystem` está **desativado**?
4. Clique numa carta: `ResolutionStep` muda para `Selected` e depois avança pelos delays do `BattleTiming`.
5. Se a regra estiver errada (não a arte), rode os testes EditMode **antes** de mexer no `CardComparer`.

```text
dotnet test tests/RockPaperPistol.Core.Tests/RockPaperPistol.Core.Tests.csproj
```

(Se o projeto de teste do `dotnet` não estiver no repo, use a janela Test Runner do Unity → EditMode.)

---

## 5. O que não é um ScriptableObject de pistola

Não existe asset “Pistola do Pistoleiro”. A pistola do jogador nasce em `DefaultCatalog.PlayerPistol` → `Card.CreatePistol(PistolId.Pistoleiro)`.

As pistolas dos inimigos são só o campo `Pistol` nos SOs de `Content/Enemies`. As regras (bônus, naipe mutável, curva da Múmia) estão em `CardComparer.cs`.
