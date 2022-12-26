# Client.Angular

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 1.7.4.  
You will need the server/backoffice application running for the client to work.

## Development server

Run `ng serve` for a dev server.  
Navigate to `http://localhost:4200/`.  
The app will automatically reload if you change any of the source files.
  
Run `ng serve --configuration=pt` for a localized version.

### for PWA tests
Run `ng build --prod`
Then `http-server -p 4200 -c-1 dist`

## To extract locale terms

First run `ng xi18n --output-path locale` for the view localization.  
Then run `./node_modules/.bin/ngx-extractor -i src/**/*.ts -f xlf -o src/locale/messages.en.xlf` for the dynamic terms.  
See https://github.com/ngx-translate/i18n-polyfill for help.

## To modify/build CMS Capture Templates

Work on the /projects/capture-templates project.
Build it using `ng build capture-templates`.
Then copy _dist/capture-templates/bundles/capture-templates.umd.min.js_ to _/src/app/assets/plugins_.

## To modify/build Box Material

Build it using `ng build box-material`.

## Authentication

The client will try to authenticate using OAuth at the server (localhost:5000).  
The following type-script library is used to handle the client authentication:
https://github.com/manfredsteyer/angular-oauth2-oidc


## Create module
- ng g m 'your name module'

## Create component
- ng g c --name 'your name module'

## Client Deploy
Run `ng build --base-href /client/ --configuration=stage-pt`
Run `ng build --base-href / --configuration=production-pt`

## Local Deploy
ng build --configuration=stage-pt
ng build --configuration=production-pt



https://stackoverflow.com/questions/46809626/angular-no-module-factory-available-for-dependency-type-contextelementdependenc
1 - rm -R node_modules (remove node_modules folder)
2 - npm i -g webpack
3 - npm i -g webpack-dev-server
4 - remove package-lock.json (if it's there)
5 - npm i
6 - npm start


## Install mask
npm install ngx-mask@8.0.0
npm i angular2-text-mask --save
npm install chart.js --save
npm install ng2-charts@~2.2.5 --save


## Run for a localized version.
ng serve --configuration=pt