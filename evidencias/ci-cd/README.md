# Evidencias CI/CD — verificación técnica U4 y pendientes de entrega

Preparación local no equivale a despliegue Azure Pipelines verificado.
Las imágenes de `../azure/` y `../iis/` pertenecen a la entrega histórica.
Guía: [configuración CI/CD](../../docs/CI_CD.md).

## U4 / T2 — cambio visible verificado públicamente

Antecedente remoto real: run `20261121.2`, SHA
`f0ea27f305959f4bbd4d02a96dce7339d36fa918`, BuildId `2`, CI/CD verdes
según evidencia previa; 100 % aprobado, conteo de pruebas de ese run desconocido.
No atribuirle las 17 pruebas locales ni confundirlo con un nuevo despliegue U4.

Baseline público leído el `2026-11-21T03:30:54.462619+00:00`:
[`u4-antes.json`](u4-antes.json) conserva URL, instantes UTC, status 200 de
`/` y `/build-info.json`, HTML completo, H1 `Cada reporte cuenta.` e identidad
SHA/BuildId coincidentes con el antecedente.
Captura antes disponible: [`u4-antes.png`](u4-antes.png), copia byte-exacta de
`Images/imagen12.png` del repositorio académico, revisada por el padre: URL Azure,
H1 antiguo y aviso académico visibles. SHA256 origen/destino coincidente:
`b66bc6f1558c864ef484843084fd5ef8f101d41a62417e46146589266bfaa9ab`.
No se modificó el original. Captura después disponible:
[`u4-despues.png`](u4-despues.png), copia de `Images/imagen14.png` realizada y
verificada por el padre (cmp exit 0), con URL Azure, H1 nuevo y aviso académico.
SHA256 reportado por el padre:
`ad9c26415b77991c31b2dc065d9c6ae99a568f346765d96fef1de012c61a796c`.
Original académico intacto.

Comandos locales observados (timeout de 30 s, sin timeout alcanzado):
- `dotnet test DockerDucks.sln --configuration Release --no-restore --filter FullyQualifiedName~Home_serves_the_rescue_interface_in_production`:
  RED exit 1, 2 fallidas/0 aprobadas por ausencia del nuevo H1 exacto;
  tras cambiar solo el H1, GREEN exit 0, 2 aprobadas/0 fallidas/0 omitidas.
- `dotnet test DockerDucks.sln --configuration Release --no-restore`:
  exit 0, 17 aprobadas/0 fallidas/0 omitidas.

Se amplió la teoría existente para `/` y `/index.html`, comprobando H1 y aviso
académico completo. Se conservan las pruebas de assets y ruta API desconocida.
### Verificación remota U4

[`u4-run.png`](u4-run.png) es copia byte-exacta de `Images/imagen13.png`;
cmp exit 0 y SHA256 origen/destino
`7b77231a17eda2cc6985e44d5846ac5eeee3625a75828176e9bff94b5af76311`.
Según revisión del padre: run `20261121.3`, `Rolling build triggered`, main
`e5151812`, CI/CD verdes, 100 % de tests, 1 artefacto; total 2m48s,
CI 1m54s, CD 40s. El porcentaje por sí solo no demuestra el conteo;
la captura de detalle [`u4-tests.png`](u4-tests.png), revisada por el padre,
confirma para `20261121.3`: 17 Total, 17 Passed, 0 Failed, 0 Others,
0 Tests not reported, 100 %, duración 2s617ms; 1 Test Run Completed/Passed.
Es copia byte-exacta de `Images/imagen15.png`, cmp exit 0, SHA256 común
`676fc7a95c7821b98a21d60b407f7c96079b6b838b1af19c77d54382356034a9`.
El conteo del run `20261121.2` sigue desconocido.

### ZIP descargado inspeccionado, sin extracción ni ejecución

[`u4-artefacto.json`](u4-artefacto.json) registra inspección con Python stdlib
ZipFile (exit 0) de `animal-rescue-run-3.zip` a
`2026-11-21T03:53:56.773086+00:00`: 896652 bytes, SHA256
`75f85214b55a1ddbda98c45d2a023dc20d5cb4550b7657ccba2bf02adcebb6dc`.
Es directamente el paquete, no un wrapper con ZIP anidado: 17 entradas,
2973830 bytes expandidos, CRC correcto, sin rutas absolutas/`..`, duplicados
normalizados ni symlinks; dentro de límites <50 MiB y <2000 entradas.
DLL, web.config, .deps.json, .runtimeconfig.json, wwwroot/index.html y
wwwroot/build-info.json están en sus rutas de raíz esperadas. H1 nuevo exacto,
aviso académico, SHA esperado y BuildId `3` coinciden. El manifiesto conserva
inventario de rutas, tamaños y CRC; no se añadió ZIP ni binarios a Git.

GET reales mediante Python stdlib urllib, cada petición con timeout 6 s,
sin polling ni reintentos: exit 0. [`u4-despues.json`](u4-despues.json) conserva
respuestas completas, URL, UTC y checks. `/` observado
`2026-11-21T03:42:45.000512+00:00`; `/health`, `/` y `/build-info.json`
devuelven 200. Health `Healthy`, H1 `Cada reporte cuenta. Cada rescate importa.`,
aviso académico presente, SHA `e515181264bd26c601e56c64ed4d45105af8259a` y
BuildId observado `3`. Todos los checks guardados pasan.

U4 funciona técnicamente según captura del run y GET público coincidente.
[Run 20261121.3](https://dev.azure.com/dockerducks-animal-rescue/AnimalRescue/_build/results?buildId=3&view=results):
URL real suministrada por el usuario. La entrega no está finalizada: faltan
el enlace directo del artefacto y la comprobación de acceso docente al proyecto
privado; disponer del enlace del run no demuestra ese acceso. No se redactaron
guiones ni se hicieron commit/push desde esta unidad.

## Checklist por completar con evidencia real

- [ ] SHA de main y contribuciones reales de cada integrante, sin atribuir
  contribuciones futuras usando la tabla histórica.
- [ ] URL del run Azure DevOps y BuildId; trigger automático tras cambio main.
- [ ] CI Windows/.NET 8: restore, build Release y resultado de tests.
- [ ] Artefacto `animal-rescue-web`, ZIP `animal-rescue.zip` descargable;
  DLL, web.config, wwwroot y build-info.json sin carpeta contenedora.
- [ ] CD/environment `animal-rescue-azure`: éxito y descarga del mismo run,
  sin checkout/rebuild; AzureWebApp Windows zipDeploy.
- [ ] URL pública real, `/health`, `/` y JSON con SHA/BuildId del mismo run;
  captura de logs del smoke exitoso, no solo página pública histórica.
- [ ] Video 1: enlace accesible y participación exigida por la rúbrica.
- [ ] Video 2: enlace accesible y participación exigida por la rúbrica.
- [ ] PDF final con enlaces, resultados y evidencias exigidas por la rúbrica.

Los guiones de ambos videos se prepararán **después de verificar** la
implementación remota y completar la evidencia pendiente. No hay enlaces ficticios;
la evidencia U4 nueva se distingue del baseline anterior. Revisar accesibilidad para el docente y ocultar credenciales,
identificadores personales innecesarios, tokens y pantallas de autenticación.

Validación local ya realizada: 17 tests Release aprobados, publicación
framework-dependent, estructura ZIP y smoke del paquete extraído con identidad
local. Ver resultados y límites en la guía; no acreditan CD Azure.
