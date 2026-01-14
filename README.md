# Environment Launcher

Breve guida rapida per l'uso del progetto `EnvironmentLauncher`.

Scopo
- Permettere di impostare variabili d'ambiente a livello utente/temporaneo e lanciare tool con override d'ambiente per quel processo.

Principi di funzionamento
- Tab `Environment`: permette di abilitare/disabilitare e modificare variabili che saranno applicate al processo al momento del lancio (override temporaneo). `Reset variabili` rilegge i valori correnti dell'utente e aggiorna la grid.
- Pulsante `Salva Variabili`: salva la configurazione in `config.json` e può opzionalmente applicare le variabili come variabili utente persistenti.
- `Avvia`: lancia il tool selezionato nella lista a sinistra. Le variabili abilitate nella grid sono sempre applicate al processo lanciato (per-process), indipendentemente dal flag `UseEnvironment` del tool.
- Tab `Tools`: modifica la lista dei tool (nome, eseguibile, argomenti, working dir). Le nuove voci hanno `UseEnvironment` impostato di default a `true`.
- Tab `Versions`: ricerca installazioni di JDK/Maven/Node e permette di impostare rapidamente `JAVA_HOME`/`MAVEN_HOME`/`NODE_HOME` nella grid; aggiorna anche la PATH utente quando richiesto.

Note tecniche
- Il servizio che lancia i processi (`Services/ToolLauncherService.cs`) applica le variabili solo per il processo avviato (non modifica l'ambiente di sistema della macchina) e ora applica sempre le variabili abilitate presenti nella grid.
- La configurazione è salvata in `config.json` nella cartella dell'eseguibile.

Comandi utili
- Build e run (da cartella `EnvironmentLauncher\EnvironmentLauncher`):

```powershell
dotnet build
dotnet run --project "EnvironmentLauncher.csproj"
```

- Eseguire il test automatico che verifica l'isolamento delle variabili:

```powershell
dotnet run --project "EnvironmentLauncher.csproj" -- --run-tests
```

File principali
- `Forms/MainForm.cs` — interfaccia principale (tabs: Environment, Tools, Versions)
- `Services/ToolLauncherService.cs` — logica di avvio processi e applicazione environment
- `Services/ConfigurationService.cs` — persistenza JSON (`config.json`)
- `Models/EnvironmentVariable.cs`, `Models/ToolConfig.cs` — modelli dati

