// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {

  production: false,

  appName: 'RadioDados Analítica',

  get IDENTITY_SERVER_URL() {
    return 'http://localhost:5000';
  },

  get API_SERVER_URL() {
    return this.IDENTITY_SERVER_URL + '/api';
  },

  get CLIENT_URL() {
    return window.location.origin;
  },

  get URL_MAPS() {
    // return 'https://www.google.com/maps/search/?api=1&query=';
    return 'https://www.google.com/maps/@?api=1&map_action=map&center=';
  },

  REQUIRE_HTTPS: false

};
