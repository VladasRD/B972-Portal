export const environment = {

  production: true,

  appName: 'RadioDados Analítica',

  get IDENTITY_SERVER_URL() {
    // FOR PRODUTCION ENVIROMENT WHERE THE IDENTITY IS AT THE SAME HOST THAT THE CLIENT
    return 'https://rdportal.com.br';
  },

  get API_SERVER_URL() {
    // FOR PRODUTCION ENVIROMENT WHERE THE API IS AT THE SAME HOST THAT THE CLIENT
    // return window.location.protocol + '://' +  this.window.location.hostname + '/api';
    return this.IDENTITY_SERVER_URL + '/api';
  },

  get CLIENT_URL() {
    // return this.IDENTITY_SERVER_URL;
    return window.location.origin;
  },

  get URL_MAPS() {
    // return 'https://www.google.com/maps/search/?api=1&query=';
    return 'https://www.google.com/maps/@?api=1&map_action=map&center=';
  },

  REQUIRE_HTTPS: false,

  TIMEOUT_REQUEST_DASHBOARD: 30000
};
