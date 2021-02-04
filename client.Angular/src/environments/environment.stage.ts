export const environment = {

  production: false,

  appName: 'RadioDados Analítica',

  get IDENTITY_SERVER_URL() {
    return 'URL_STAGE';
  },

  get API_SERVER_URL() {
    // return window.location.protocol + '://' +  this.window.location.hostname + '/api';
    return this.IDENTITY_SERVER_URL + '/api';
  },

  get CLIENT_URL() {
    return window.location.origin;
  },

  get URL_MAPS() {
    return 'https://www.google.com/maps/@?api=1&map_action=map&center=';
  },

  REQUIRE_HTTPS: false

};
