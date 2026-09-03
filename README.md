# Rock Paper Pistol

Primeiro Projeto do Lumo Lab 3

Jogo de batalha de cartas 2D em primeira pessoa, feito pela **Lumo Lab**. O Pistoleiro chega a um parque de diversões onde as estátuas ganham vida à noite e disputa o título de MVS — *Most Valuable Statue*.

Esta entrega contém o **núcleo jogável em C#**: comparação das cartas, gestão de baralho e a run contra três oponentes. Sem arte polida. Serve para validar a mecânica no Unity e para o time de design tunar os dados.

## Como jogar o playtest

1. Abra esta pasta no **Unity 2022.3 LTS** ou **Unity 6** (projeto 2D).
2. Espere o Unity importar os scripts.
3. Aperte **Play** numa cena vazia. A UI de debug sobe sozinha.
4. Escolha um dos três baralhos (essa escolha não muda na run).
5. Enfrente, nesta ordem: **Estátua de Pedra** → **Múmia** → **Pirata**.

Controle do playtest: point and click nos botões da mão.

Para recriar os ScriptableObjects padrão no Editor: **Rock Paper Pistol → Gerar conteúdo padrão**.

## Como rodar os testes

O núcleo não depende do Unity. Na raiz do repositório:

```powershell
dotnet test RockPaperPistol.sln
```

No Unity, os mesmos testes ficam em `Assets/Scripts/Tests/EditMode` (Test Framework, Edit Mode).

## Mecânica

Cada carta tem **naipe** (Pedra, Papel, Tesoura) e **número** (1 a 5).

- Papel vence Pedra, Pedra vence Tesoura, Tesoura vence Papel.
- Quem tem vantagem de naipe recebe **+1** no número daquela comparação.
- Se o número ajustado empatar, o naipe decide.
- Se naipe e número forem iguais: empate. Ninguém pontua e o **próximo round vale +1** (acumula se empatar de novo).
- Uma comparação por round. A carta jogada vai ao **descarte** e não volta neste encontro.

Exemplos (iguais ao GDD):

- Pedra 1 vs Papel 1 → Papel vira 2 → Papel ganha
- Pedra 2 vs Pedra 3 → Pedra 3 ganha
- Tesoura 2 vs Tesoura 2 → empate, próximo round vale +1
- Papel 3 vs Tesoura 1 → Tesoura vira 2 → Papel 3 ganha

### Baralho

- 9 cartas no baralho escolhido; mão de 3.
- Cada round: joga 1 da mão, descarta, compra 1 se ainda houver carta na pilha.
- Em 5 rounds o jogador usa **5 de 9**. As quatro que ficam de fora são a decisão.
- Ao vencer um oponente, o baralho **volta completo** para o próximo encontro.
- Placar: precisa ter **mais pontos** que o oponente. Empate no placar é derrota.

### Personagens

| Papel | Personagem | Comportamento |
| --- | --- | --- |
| Jogador | Pistoleiro | Escolhe o baralho |
| Oponente 1 | Estátua de Pedra | Defensivo (baralho de Pedra) |
| Oponente 2 | Múmia | Defensivo (Tesoura que prende Papel) |
| Oponente 3 | Pirata | Agressivo (números altos mistos) |

O jogador **não escolhe** os oponentes. A sequência é fixa. Cada oponente joga 5 cartas numa ordem scriptada (dá para aprender no replay).

### Baralhos do Pistoleiro

- **Equilibrado** — cobertura dos três naipes, picos médios.
- **Agressivo** — números altos, buraco em Tesoura.
- **Contrário** — Tesoura forte, Papel fraco.

Os valores ficam em `Assets/Content/` (`DeckDefinition` e `EnemyDefinition`). Dá para tunar sem mexer na regra.

## Estrutura

```
Assets/Scripts/Core/      regras puras (sem UnityEngine)
Assets/Scripts/Data/      ScriptableObjects
Assets/Scripts/Unity/     playtest (GameSessionDriver + DebugPlayView)
Assets/Scripts/Tests/     testes Edit Mode
Assets/Scripts/Editor/    menu que gera o conteúdo padrão
Assets/Content/           3 baralhos + 3 oponentes
src/                      projeto .NET que compila o Core
tests/                    NUnit fora do Unity
```

Fluxo do código: `DebugPlayView` → `GameSessionDriver` → `RunSession` → `Encounter` + `DeckRuntime` → `CardComparer`.

## Fora desta entrega

Arte das cartas, tela do parque, efeito da carta Pistol, IA ponderada e sudden death em empate de encontro.

## Time (GDD)

Lumo Lab — produção, programação, arte 2D, game design e narrativa conforme o documento *GDD - Rock Paper Pistol*.
