export const environment = {

  production: true,

  appName: 'RadioDados Analítica',

  get IDENTITY_SERVER_URL() {
    return 'MSD_PRODUCTION_URL';
  },

  get API_SERVER_URL() {
    // return window.location.protocol + '://' +  this.window.location.hostname + '/api';
    return this.IDENTITY_SERVER_URL + '/api';
  },

  get CLIENT_URL() {
    return this.IDENTITY_SERVER_URL;
  },

  get URL_MAPS() {
    return 'https://www.google.com/maps/@?api=1&map_action=map&center=';
  },

  REQUIRE_HTTPS: true
};
