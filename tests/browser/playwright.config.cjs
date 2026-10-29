const { defineConfig } = require('playwright/test');

module.exports = defineConfig({
  testDir: '.',
  testMatch: '*.spec.cjs',
  workers: 1,
  use: { baseURL: process.env.BASE_URL || 'http://127.0.0.1:5187' },
  webServer: process.env.BASE_URL ? undefined : {
    command: 'dotnet run --project src/DockerDucks.Api --configuration Release --no-launch-profile --urls http://127.0.0.1:5187',
    cwd: '../..',
    url: 'http://127.0.0.1:5187/health',
    env: { ASPNETCORE_ENVIRONMENT: 'Production' },
    reuseExistingServer: false
  }
});
