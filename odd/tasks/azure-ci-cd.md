# Animal Rescue — preparación local de CI/CD

## Objetivo y límites

Preparar Azure Pipelines CI → artefacto → CD para la práctica final. La rúbrica requiere cambios en main, build/pruebas, artefacto reutilizable y despliegue automático en App Service. No afirmar que el YAML se ejecutó en Azure DevOps sin evidencia real. No hacer commit, push, crear recursos ni autenticarse en servicios durante esta unidad.

## T1 — Pipeline y guía de configuración

- Crear `azure-pipelines.yml`: trigger main, CI .NET 8 en Windows, restore/build Release/test (tests existentes), publish framework-dependent y ZIP sin carpeta contenedora; publicar artefacto `animal-rescue-web` con `animal-rescue.zip`.
- CI agrega `wwwroot/build-info.json` al contenido publicado con SHA y BuildId (no secretos). Validar DLL, web.config y wwwroot antes de publicar.
- CD depende del éxito de CI y solo corre en main/no PR; deployment job en environment `animal-rescue-azure`, descarga explícita del mismo run y sin checkout/build. AzureWebApp@1, Windows (`webApp`), zipDeploy.
- Conexión propuesta `sc-animal-rescue-azure`. App Service histórico `animal-rescue-dockerducks`, URL `https://animal-rescue-dockerducks-g0a8a8fteycfc7hc.westus-01.azurewebsites.net`. Deben comprobarse antes de activar CD.
- Smoke checks acotados de `/health`, `/` y `/build-info.json` comparando SHA y BuildId con ejecución; fallar si no coincide.
- Crear `docs/CI_CD.md` con pasos reales de configuración DevOps/GitHub/agentes, environment, service connection Azure ARM preferentemente federada con permiso restringido a pipeline, permisos, evidencia y limitaciones. Conservar entrega anterior sin atribuir contribuciones futuras.
- Crear `evidencias/ci-cd/README.md` con checklist pendiente de SHA/run/artefacto/CD/URL y ambos videos/PDF. Guiones después de implementación remota verificada.
- Modificar README raíz mínimamente para señalar nueva práctica en preparación y enlazar guía; marcar lo anterior como antecedente sin borrar evidencias ni datos.

## Superficies permitidas

`azure-pipelines.yml`, `docs/CI_CD.md`, `evidencias/ci-cd/README.md`, `README.md`. No modificar código ni tests de aplicación.

## Comprobaciones

- Fuente oficial: https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-web-app-v1?view=azure-pipelines y https://learn.microsoft.com/en-us/azure/devops/pipelines/library/connect-to-azure?view=azure-devops.
- YAML/tasks/PowerShell deben revisarse; si no hay validador o pwsh local indicarlo sin fingir ejecución.
- Local dotnet test Release ya pasó con 17 pruebas en revisión previa. Validar publicación y estructura ZIP y, si es posible, servir publicación local para /health y estáticos; estos resultados NO acreditan CD Azure.
- RDD desactivado en sesión padre; verificar comportamiento y estructura de forma proporcionada. No usar comandos destructivos.

## Estado

T1 implementada localmente: `azure-pipelines.yml`, `docs/CI_CD.md`,
`evidencias/ci-cd/README.md` y enlace mínimo en README con entrega anterior
explícitamente histórica. ASSESS completado: RDD desactivado, verificación local del escritor suficiente y sin verificador independiente requerido. Sin revisión nativa.

Evidencia local observada:
- `dotnet test DockerDucks.sln --configuration Release --no-restore`: 17
  aprobadas, 0 fallidas, 0 omitidas.
- `dotnet publish src/DockerDucks.Api/DockerDucks.Api.csproj --configuration
  Release --no-restore --self-contained false --output
  /tmp/animal-rescue-local-check/publish`: exit 0.
- Python stdlib: ZIP con DLL/web.config/wwwroot en raíz relativa, extracción,
  servidor propio y smoke `/health` 200 Healthy, `/` 200 HTML,
  `/build-info.json` 200 con SHA de HEAD y BuildId local-validation exactos;
  identidades alternativas distintas no coinciden. Servidor detenido en finally.
- Padre reportó PyYAML PASS para contratos trigger/tareas/ZIP/artefacto/gates/
  identidad. Agregado `trigger.batch: true` solicitado; editor reporta string
  esperado, aunque Azure define boolean. No disponible pwsh: no se ejecutaron
  scripts PowerShell ni tasks de Azure. RDD/Strict TDD no activos.

La fase remota DevOps/Azure requiere configuración y autorización real antes
de poder declararse funcional. La respuesta pública actual verificada por el
padre no acredita permisos/configuración Azure ni CD. No hubo commit, push,
autenticación, creación remota ni deploy.
