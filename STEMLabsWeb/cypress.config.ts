import { defineConfig } from "cypress";
import dotenv from "dotenv";
const env = dotenv.config({ path: "./.env.local" }).parsed;

export default defineConfig({
  projectId: "v9jdfn",
  e2e: {
    setupNodeEvents() {},
    experimentalRunAllSpecs: true,
  },
  env: {
    ...env,
  },
});
