# Evidencias CI/CD — pendientes de ejecución remota

Preparación local no equivale a despliegue Azure Pipelines verificado.
Las imágenes de `../azure/` y `../iis/` pertenecen a la entrega histórica.
Guía: [configuración CI/CD](../../docs/CI_CD.md).

## U4 / T2 — cambio visible validado solo localmente

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
No se generó una captura nueva ni se modificó el original. La captura después
todavía no existe y sigue pendiente junto con push, nuevo run y CD.

Comandos locales observados (timeout de 30 s, sin timeout alcanzado):
- `dotnet test DockerDucks.sln --configuration Release --no-restore --filter FullyQualifiedName~Home_serves_the_rescue_interface_in_production`:
  RED exit 1, 2 fallidas/0 aprobadas por ausencia del nuevo H1 exacto;
  tras cambiar solo el H1, GREEN exit 0, 2 aprobadas/0 fallidas/0 omitidas.
- `dotnet test DockerDucks.sln --configuration Release --no-restore`:
  exit 0, 17 aprobadas/0 fallidas/0 omitidas.

Se amplió la teoría existente para `/` y `/index.html`, comprobando H1 y aviso
académico completo. Se conservan las pruebas de assets y ruta API desconocida.
U4 sigue local: pendientes push del padre, nuevo run automático, artefacto, CD y
GET público con nuevo H1 y SHA/BuildId de ese mismo run. No hay CD nueva acreditada.

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
implementación remota. No hay enlaces ficticios ni evidencia de CD en esta
carpeta para U4. El baseline anterior no acredita un nuevo CD. Revisar accesibilidad para el docente y ocultar credenciales,
identificadores personales innecesarios, tokens y pantallas de autenticación.

Validación local ya realizada: 17 tests Release aprobados, publicación
framework-dependent, estructura ZIP y smoke del paquete extraído con identidad
local. Ver resultados y límites en la guía; no acreditan CD Azure.
