# PA Case Study

Unity playable case study for a single-song Duet Cats rhythm interaction.

## Scope

- JSON chart content is validated and mapped to `SongContent` / `RuntimeNote`.
- `CatInput` drives two cat lanes; `NoteSystem` spawns notes; `JudgementSystem` resolves hits and misses; `ScoreState` owns score updates.
- Intro/onboarding and combo feedback are implemented.
- Fever after a successful special-note hit is a proposed improvement: a timed state that increases gameplay speed.
- Ripple hit feedback is temporarily disabled on web because of a web-build issue.

## Project Layout

```text
Assets/_Game/
├── Config/       ScriptableObject tuning and feedback catalogs
├── Content/      BabyMonster chart content
├── Prefabs/      Gameplay prefabs
├── Scenes/       Game scene entry
├── Scripts/      Runtime systems by responsibility
└── Shaders/      Ripple and iris shaders/materials
```

## Documentation

- [Technical Overview](DuetCats_Technical_Overview.md)
- [Vietnamese Technical Overview](DuetCats_Technical_Overview.vi.md)
