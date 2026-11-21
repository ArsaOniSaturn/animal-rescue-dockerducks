# CI/CD Azure Pipelines — guía y estado verificado

**U4 verificada técnicamente; entrega documental en curso.**
[Run automático 20261121.3](https://dev.azure.com/dockerducks-animal-rescue/AnimalRescue/_build/results?buildId=3&view=results):
CI/CD verdes, 17 tests remotos aprobados, 0 fallidos; SHA
`e515181264bd26c601e56c64ed4d45105af8259a`, BuildId `3`.

| Comprobación | Evidencia |
| --- | --- |
| ZIP real, 896652 bytes: CRC, raíz, H1 y SHA/BuildId PASS | [Manifiesto](../evidencias/ci-cd/u4-artefacto.json) |
| GET públicos: 200, Healthy, H1 nuevo e identidad coincidente | [Respuestas reales](../evidencias/ci-cd/u4-despues.json) |
| Capturas antes/después, run y 17/17 tests | [Índice de evidencias](../evidencias/ci-cd/README.md) |

La evidencia corresponde a ese SHA, no al futuro HEAD: publicar documentación
activará otro run. Pendientes: enlace directo del artefacto, acceso docente al
proyecto privado y GitHub `oalarconpe` (no confirmados), ambos videos, PDF y Teams.
[Reparto actual acordado](../README.md#equipo-y-responsabilidades-acordadas-para-cicd):
todos revisan y participan en ambos videos; no implica autoría retrospectiva.
No se redactan guiones todavía. La entrega IIS/Azure anterior sigue siendo histórica.

## Contrato del pipeline

`azure-pipelines.yml` dispara CI con cambios en `main`, sin trigger PR.
`batch: true` agrupa pushes mientras hay un run activo; no serializa runs
manuales, por lo que se debe evitar desplegarlos concurrentemente.
Agente Microsoft-hosted `windows-latest`, SDK 8.x mediante UseDotNet@2;
DotNetCoreCLI@2 restaura solución, compila Release, ejecuta tests existentes
y publica framework-dependent, sin runtime autocontenido. ArchiveFiles@2
comprime el contenido, no la carpeta. PublishPipelineArtifact@1 conserva
`animal-rescue-web/animal-rescue.zip`.

Antes de empaquetar se exigen DLL, web.config y wwwroot/index.html y se agrega
`wwwroot/build-info.json` con `sha` y `buildId` del run, sin secretos.
CD depende de CI exitoso y exige main/no PR. El deployment job usa environment
`animal-rescue-azure`, sin checkout ni rebuild; desactiva descarga implícita y
DownloadPipelineArtifact@2 descarga explícitamente el artefacto del run actual.
AzureWebApp@1 usa Windows (`webApp`) y `zipDeploy`.

Smoke consulta `/health` (200 y Healthy), `/` (200 y HTML) y `/build-info.json`
(200, JSON y coincidencia exacta de SHA/BuildId). Hasta 12 intentos, tres
peticiones con timeout de 15 s e intervalo de 10 s: máximo aproximado 650 s
más overhead. Una versión vieja saludable **no** satisface la identidad.
El JSON es público: no agregar secretos. Una falla del smoke falla CD pero
no hace rollback; investigar y volver a desplegar un run aprobado.

## Configuración remota — referencia para revisión o reproducción

El run U4 ya se ejecutó; estos pasos describen el procedimiento, no una lista
íntegramente pendiente ni una auditoría de permisos privados.

1. Confirmar repositorio GitHub, rama `main`, permisos del equipo y política
   de cambios. Crear proyecto Azure DevOps y pipeline YAML desde repositorio
   existente, seleccionando `/azure-pipelines.yml`. Autorizar únicamente el
   repositorio necesario mediante integración GitHub/Azure Pipelines; una
   conexión GitHub no sustituye la conexión Azure ARM. No subir credenciales.
2. Comprobar disponibilidad/cuota de agentes Microsoft-hosted y paralelismo
   del proyecto; solicitar habilitación si la organización nueva no tiene
   minutos. El YAML requiere agente Windows con acceso a NuGet y Azure.
3. Antes de activar el pipeline/CD, confirmar suscripción, tenant, resource
   group, propiedad y vigencia de `animal-rescue-dockerducks`, Windows,
   runtime .NET 8, HTTPS Only y `ASPNETCORE_ENVIRONMENT=Production`.
   Confirmar que la URL en `appUrl` corresponde a ese recurso:
   https://animal-rescue-dockerducks-g0a8a8fteycfc7hc.westus-01.azurewebsites.net
   Los GET U4 comprobaron HTML, `/health=Healthy` e identidad SHA/BuildId;
   junto con la captura del run acreditan el resultado técnico de U4, pero no
   constituyen auditoría de permisos ni configuración ARM.
4. En Project settings → Service connections, crear Azure Resource Manager
   `sc-animal-rescue-azure`, preferentemente Workload Identity Federation.
   Seleccionar la suscripción/tenant correctos y limitar RBAC al recurso Web
   App (o mínimo scope viable, documentando justificación). Verificar permiso
   de despliegue y acceso al endpoint SCM; no habilitar credenciales básicas
   ni abrir redes indiscriminadamente para sortear errores.
   Autorizar solo este pipeline, no “Grant access to all pipelines”. La persona
   configuradora necesita permisos para crear conexión y asignar RBAC;
   si carece de ellos, debe intervenir una persona autorizada de la cuenta.
5. Crear environment `animal-rescue-azure` en Pipelines → Environments,
   restringir pipeline permissions a este pipeline y definir responsables.
   Para despliegue automático no agregar aprobación manual obligatoria;
   si la política institucional la exige, documentar que CD quedará esperando.
   Autorizar environment y service connection antes de exigir automatismo.
6. Revisar variables YAML propuestas; no suponer que el recurso histórico
   sigue siendo correcto. Solo entonces habilitar ejecución y realizar cambio
   autorizado en main. Esta unidad no hace commit, push, login, creación
   remota ni despliegue. Verificar CI, tests publicados, ZIP descargable,
   download del mismo run, AzureWebApp y smoke en logs.
7. Conservar SHA, BuildId, URL del run, nombre del artefacto, resultado del
   environment y JSON público. Si falla autorización, cuota, red o identidad,
   corregir configuración y repetir: no declarar éxito solo por HTTP 200.
   Consultar [checklist de evidencias](../evidencias/ci-cd/README.md).

## Antecedente T1 — validación local y límites

- `dotnet test DockerDucks.sln --configuration Release --no-restore`:
  **17 aprobadas, 0 fallidas, 0 omitidas**.
- `dotnet publish src/DockerDucks.Api/DockerDucks.Api.csproj --configuration
  Release --no-restore --self-contained false --output
  /tmp/animal-rescue-local-check/publish`: exit 0; DLL/web.config/wwwroot presentes.
- Python stdlib empaquetó ZIP sin carpeta raíz, comprobó estructura/JSON,
  extrajo y sirvió ese paquete en `127.0.0.1:5188`. `/health` → 200 Healthy,
  `/` → 200 HTML, `/build-info.json` → 200 con SHA
  `be5e1fe9452029e8f5d98e568d63aa5bbe5b83cc` y BuildId `local-validation`.
  Identidades alternativas SHA/BuildId distintas no coinciden. Se detuvo
  únicamente el servidor propio mediante finally. No se modificaron fuentes.
- En aquella validación, la identidad local representaba HEAD previo, no un
  run Azure; el YAML aún no estaba publicado. El ZIP local usaba Python,
  no ArchiveFiles@2. No confundir este antecedente con la evidencia U4 superior.
- Se validaron con PyYAML los contratos main, tareas, ZIP sin raíz,
  artefacto del mismo run, gates y smoke SHA/BuildId: PASS. Tras agregar
  `batch: true`, el editor espera erróneamente string en esa propiedad;
  Azure Pipelines define batch como boolean. No se cambia a string para
  satisfacer ese diagnóstico. No hay `pwsh` local: PowerShell y tasks
  oficiales no se ejecutaron aquí. Un parser no sustituye validación de Azure
  ni prueba del agente Windows.
- En la validación local T1 no se revalidó la entrega histórica ni se realizó
  autenticación en Azure/GitHub; no describe el estado remoto posterior de U4.

## Referencias oficiales para configuración y revisión

- [AzureWebApp@1: inputs y deploymentMethod](https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-web-app-v1?view=azure-pipelines)
- [Conexión Azure ARM y federación](https://learn.microsoft.com/en-us/azure/devops/pipelines/library/connect-to-azure?view=azure-devops)
- [DownloadPipelineArtifact@2: current](https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/download-pipeline-artifact-v2?view=azure-pipelines)
- [ArchiveFiles@2](https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/archive-files-v2?view=azure-pipelines)
