import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from '../environments/environment';

export const authConfig: AuthConfig = {

  // Url of the Identity Provider
  issuer: environment.IDENTITY_SERVER_URL,

  // URL of the SPA to redirect the user to after login
  redirectUri: environment.CLIENT_URL + '/index.html',

  // URL of the SPA to redirect the user after silent refresh
  silentRefreshRedirectUri: environment.CLIENT_URL + '/silent-refresh.html',

  // The SPA's id. The SPA is registerd with this id at the auth-server
  clientId: 'js',

  // set the scope for the permissions the client should request
  // The first three are defined by OIDC. The 4th is a usecase-specific one
  scope: 'openid profile email role box',

  // silentRefreshShowIFrame: true,
  // silentRefreshTimeout: 5000, // For faster testing

  showDebugInformation: true,

  sessionChecksEnabled: false,

  // the default is 'remote-only'
  requireHttps: environment.REQUIRE_HTTPS
};
