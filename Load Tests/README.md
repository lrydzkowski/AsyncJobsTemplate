# async-jobs-template-load-tests

Load tests written in TypeScript with Grafana k6.

It contains one load test that is sending requests to API that is protected by Microsoft Entra ID. The project is using [api-authenticator](https://github.com/lrydzkowski/api-authenticator) to generate tokens necessary to authenticate.

## Prerequisites

1. Visual Studio Code
2. k6 - <https://grafana.com/docs/k6/latest/set-up/install-k6/>
3. NodeJS 22 - <https://nodejs.org/en>
4. api-authenticator - <https://github.com/lrydzkowski/api-authenticator>

## How to run it

1. Create `./config/ad-config.json` based on `./config/ad-config.json_sample`. You should fill in data necessary to get a token from Microsoft Entra ID.
2. Create `./config/tests-config.json` based on `./config/tests-config.json_sample`. You should fill in the host of tested API.
3. Create `./config/tokens.json` based on `./config/tokens.json_sample`. You don't have to fill in anything, api-authenticator will put tokens in this file.
4. Run the following commands:

   ```powershell
   npm install
   npm run test1
   ```

5. Open summary: `./results/summary.html`.
