# Animal Rescue — preparación local de CI/CD

## Objetivo y límites

Preparar y demostrar Azure Pipelines CI → artefacto → CD para la práctica final. La rúbrica requiere cambios en main, build/pruebas, artefacto reutilizable y despliegue automático en App Service. No afirmar ejecuciones remotas sin evidencia real. Las restricciones de preparación local de T1 y su estado original se conservan abajo como antecedentes. En la fase remota posterior el estudiante autorizó commit y push a main; el padre gestiona esa entrega, sin force push ni recursos facturables nuevos.

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

## T2 — Cambio visible trazable por CI/CD (en curso)

### Punto de partida confirmado

Run automático `20261121.2`, main `f0ea27f305959f4bbd4d02a96dce7339d36fa918`: CI/CD verdes, 100 % de pruebas aprobadas y un artefacto según captura `imagen11.png` en el repositorio académico. GET público `/health` devuelve `Healthy`; `/build-info.json` muestra ese SHA y BuildId `2`. No confundir número de run con BuildId. El encabezado público anterior dice `Cada reporte cuenta.`.

### Comportamiento y aceptación

- Cambiar únicamente el texto H1 de la interfaz a `Cada reporte cuenta. Cada rescate importa.`. No rediseñar, añadir dependencias ni cambiar API, datos o permisos.
- Conservar el aviso de demostración académica, datos en memoria y no contacto con emergencias, así como IDs y accesibilidad existentes.
- Añadir una prueba de integración determinista del HTML servido por la aplicación: observar RED por el texto nuevo ausente, modificar el HTML y observar GREEN; ejecutar suite Release y comprobar aviso académico. No simular peticiones externas en tests de CI.
- Antes del push guardar una evidencia pública de la versión anterior con URL, instante real, HTML/título e identidad; captura browser solo si hay runner/navegador disponible sin instalar. No inventar imágenes ni resultados.
- Registrar comandos/resultados locales y el alcance de rollback. El éxito local no demuestra CD: después del push se necesita nuevo run automático, artefacto, CD y GET público con nuevo encabezado y SHA/BuildId del mismo run.
- No redactar guiones todavía. No alterar YAML, infraestructura, secretos ni evidencias históricas.

### Superficies permitidas — T2

- `src/DockerDucks.Api/wwwroot/index.html` (solo texto H1).
- `tests/DockerDucks.Api.Tests/WebInterfaceTests.cs` (ampliar prueba de HTML servido existente para `/` y `/index.html`; no crear un fixture redundante).
- `evidencias/ci-cd/README.md` y nuevos archivos `evidencias/ci-cd/u4-antes.json`, `evidencias/ci-cd/u4-antes.png`, `evidencias/ci-cd/u4-despues.json`, `evidencias/ci-cd/u4-despues.png` para evidencia real, sin datos inventados.
- Esta sección T2 de `odd/tasks/azure-ci-cd.md` para resultados, limitaciones y estado.

El escritor trabaja sin commit ni push; el padre verifica y entrega la unidad. No marcar T2 terminada hasta comprobar el recorrido remoto nuevo y guardar su evidencia.

### Estado local T2 y rollback

Implementado únicamente el nuevo texto H1; teoría de HTML existente ampliada con
H1 exacto y aviso académico completo para `/` y `/index.html`. IDs, accesibilidad,
API y pruebas alternativas (assets y ruta API desconocida) preservados.

- RED: `dotnet test DockerDucks.sln --configuration Release --no-restore --filter FullyQualifiedName~Home_serves_the_rescue_interface_in_production`, exit 1,
  2 fallidas por H1 nuevo ausente, 0 aprobadas.
- GREEN: mismo comando después del cambio HTML, exit 0, 2 aprobadas,
  0 fallidas, 0 omitidas.
- Suite: `dotnet test DockerDucks.sln --configuration Release --no-restore`,
  exit 0, 17 aprobadas, 0 fallidas, 0 omitidas. Comandos con timeout 30 s,
  ninguno agotado.
- Baseline real en `evidencias/ci-cd/u4-antes.json`: GET público `/` observado
  `2026-11-21T03:30:54.462619+00:00`, status 200, H1 anterior;
  `/build-info.json` status 200 con SHA `f0ea27f305959f4bbd4d02a96dce7339d36fa918`
  y BuildId `2`. Se guardaron HTML completo y respuestas reales con sus instantes.
- Captura antes disponible en [`u4-antes.png`](../../evidencias/ci-cd/u4-antes.png):
  copia byte-exacta de `Images/imagen12.png` del repositorio académico, revisada
  por el padre (URL Azure, H1 antiguo y aviso académico). SHA256 origen/destino:
  `b66bc6f1558c864ef484843084fd5ef8f101d41a62417e46146589266bfaa9ab`.
  Original intacto; sin instalaciones ni captura simulada. La captura después
  todavía no existe. Push, nuevo run y CD siguen pendientes; no hay evidencia
  de un nuevo CD U4.

Rollback propuesto (no ejecutado): revertir de forma coordinada el texto H1 y
su expectativa de test mediante un cambio revisado por el padre; conservar las
evidencias históricas y el aviso académico. Un rollback remoto requeriría su
propio run CI/CD y verificación pública de identidad, no basta el cambio local.
Pendientes del padre: revisión, entrega y nuevo recorrido remoto con evidencia.
T1 y sus antecedentes no se modifican en esta unidad.
