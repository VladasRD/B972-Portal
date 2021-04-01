export const environment = {

  production: false,

  appName: 'RadioDados Analítica',

  get IDENTITY_SERVER_URL() {
    // FOR PRODUTCION ENVIROMENT WHERE THE IDENTITY IS AT THE SAME HOST THAT THE CLIENT
    return 'https://radiodadosanalitica.azurewebsites.net';
    // return 'http://rafaelestevao-001-site2.htempurl.com';
  },

  get API_SERVER_URL() {
    // FOR PRODUTCION ENVIROMENT WHERE THE API IS AT THE SAME HOST THAT THE CLIENT
    // return window.location.protocol + '://' +  this.window.location.hostname + '/api';
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
