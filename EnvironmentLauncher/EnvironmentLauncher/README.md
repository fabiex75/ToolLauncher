# Environment Launcher

Breve guida rapida per l'uso del progetto `EnvironmentLauncher`.

Scopo
- Permettere di impostare variabili d'ambiente a livello utente/temporaneo e lanciare tool con override d'ambiente per quel processo.

Principi di funzionamento
- Tab `Environment`: permette di abilitare/disabilitare e modificare variabili che saranno applicate al processo al momento del lancio (override temporaneo). `Reset variabili` rilegge i valori correnti dell'utente e aggiorna la grid.
	- Nota PATH: se imposti/modifichi la variabile `PATH` nel tab `Environment`, il launcher unisce/prependerà automaticamente i percorsi specificati alla PATH esistente per il processo lanciato (in modo da non perdere le entry di sistema/utente già presenti).
- Pulsante `Salva Variabili`: salva la configurazione in `config.json` e può opzionalmente applicare le variabili come variabili utente persistenti.
- `Avvia`: lancia il tool selezionato nella lista a sinistra. Le variabili abilitate nella grid sono sempre applicate al processo lanciato (per-process), indipendentemente dal flag `UseEnvironment` del tool.
- Tab `Tools`: modifica la lista dei tool (nome, eseguibile, argomenti, working dir). Le nuove voci hanno `UseEnvironment` impostato di default a `true`.
 - Tab `Tools`: modifica la lista dei tool (nome, eseguibile, argomenti, working dir). Le nuove voci hanno `UseEnvironment` impostato di default a `true`.
	 - Nuove colonne disponibili:
		 - `ForceNewInstanceArguments`: argomenti aggiuntivi che possono forzare l'app a lanciare una nuova istanza (es. `--user-data-dir "%TEMP%\\vscode_test"` per VS Code).
		- `KillExistingInstances`: opzione disponibile per terminare processi esistenti prima del lancio, ma NON viene impostata automaticamente. Per evitare perdita di dati, il launcher non uccide più processi esistenti automaticamente; l'opzione rimane a disposizione per configurazioni manuali.

Behaviour per tool comuni:
- Visual Studio Code: quando rilevato automaticamente, il launcher imposta `ForceNewInstanceArguments` per avviare nuove istanze (es. `--user-data-dir "%TEMP%\\vscode_profile_%USERNAME%"`) in modo che la nuova finestra erediti le variabili d'ambiente applicate al processo di lancio.
- Visual Studio: non viene più uccisa automaticamente; se necessario si può configurare manualmente `ForceNewInstanceArguments` o `KillExistingInstances` per esigenze specifiche.
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

Se vuoi, posso aggiungere un esempio di `config.json` commentato o fare un breve video/ GIF operativo della UI.
