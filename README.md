# VotePreferentiel

- bibliotheque `VotePreferentiel` (modele + fournisseur HTTP),
- worker `VotePreferentiel.Worker` (calcul du gagnant),
- tests unitaires `VotePreferentiel.Tests` (5 cas demandes).

## URL de test
Le worker utilise par defaut:
`https://raw.githubusercontent.com/JLou/dotnet-course/refs/heads/main/eval-1/votes.example.json`

## Lancer les tests
```powershell
dotnet test .\VotePreferentiel.sln
```

## Lancer le worker
```powershell
dotnet run --project .\VotePreferentiel.Worker\VotePreferentiel.Worker.csproj
```

