import { Outgoing, OutgoingViewModel } from './outgoing';
import { DeviceLocation } from './Device';
import { Firmware } from './firmware';
import { Cep, State } from './address';
import { Package } from './package';
import { Project } from './project';
import { Dashboard } from './dashboard';
import { Report } from './report';
import { Device, DeviceRegistration } from './device';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { MessageService } from '../common/message.service';
import { HttpClient } from '@angular/common/http';
import { catchError, map, tap } from 'rxjs/operators';
import { BaseService } from '../common/baseService';
import { AppUser } from '../common/appUser';
import { p } from '@angular/core/src/render3';
import { Client, ClientUser } from './client';
import { toJSON } from 'knockout';
import { MCond } from './MCond';

@Injectable({
  providedIn: 'root'
})
export class SmartGeoIotService extends BaseService {

  constructor(protected http: HttpClient, protected messageService: MessageService, protected router: Router) {
    super(messageService, router, http);
  }

  /**
  * Gets all devices.
  */
  getDevices(skip = 0, top = 0, filter = '', scope = '', out: (totalCount: number) => void = null): Observable<Device[]> {
    const url = environment.API_SERVER_URL + '/sgiDevice/?skip=' + skip + '&top=' + top + '&filter=' + filter + '&scope=' + scope;
    return this.getEntities<Device>(() => new Device(), url, out).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }

  /**
  * Gets all devices.
  */
  getDevicesFromDashboard(skip = 0, top = 0, filter = '', out: (totalCount: number) => void = null): Observable<DeviceRegistration[]> {
    const url = environment.API_SERVER_URL + '/sgiDevice/fromDashboard/?skip=' + skip + '&top=' + top + '&filter=' + filter;
    return this.getEntities<DeviceRegistration>(() => new DeviceRegistration(), url, out).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }

  /**
  * Gets all devices.
  */
  getDevicesFromFirmware(skip = 0, top = 0, filter = '', clientUId: string = null, out: (totalCount: number) => void = null): Observable<DeviceRegistration[]> {
    const url = `${environment.API_SERVER_URL}/sgiDevice/fromFirmware/?skip=${skip}&top=${top}&filter=${filter}&client=${clientUId}`;
    return this.getEntities<DeviceRegistration>(() => new DeviceRegistration(), url, out).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }

  /**
     * Gets a given firmware of device.
     * @param id The client id
     */
  getFirmware(id: string): Observable<Firmware> {
    this.messageService.isLoadingData = true;
    return this.http.get<Firmware>(`${environment.API_SERVER_URL}/sgiFirmware/${id}`)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false; }),
        catchError(this.handleErrorAndContinue('getting firmware', new Firmware()))
      );
  }

  /**
  * Gets devices by client.
  */
  getDevicesOfClient(project: string = null): Observable<DeviceRegistration[]> {
    const url = `${environment.API_SERVER_URL}/sgiDevice/ofClient/${project}`;
    return this.getEntities<DeviceRegistration>(() => new DeviceRegistration(), url, null).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }

  /**
   * Gets reports.
   */
  getReports(deviceId: string, de: string, ate: string, skip = 0, top = 0, blocked: boolean = null, reportType: number = 0, out: (totalCount: number) => void = null): Observable<Report[]> {
    const url = `${environment.API_SERVER_URL}/sgiReport/${deviceId}?de=${de}&ate=${ate}&skip=${skip}&top=${top}&blocked=${blocked}&reportType=${reportType}`;
    return this.getEntities<Report>(() => new Report(), url, out).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }

  /**
   * Gets reports MCond.
   */
   getReportMCond(deviceId: string, de: string, ate: string, skip = 0, top = 0, out: (totalCount: number) => void = null): Observable<MCond[]> {
    const url = `${environment.API_SERVER_URL}/sgiReport/B987/${deviceId}?de=${de}&ate=${ate}&skip=${skip}&top=${top}`;
    return this.getEntities<MCond>(() => new MCond(), url, out).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }
  /**
   * Export MCond reports to excel.
   */
   exportReportMCondToExcel(deviceId: string, de: string, ate: string, top = 0) {
    this.messageService.isLoadingData = true;
    const url = `${environment.API_SERVER_URL}/sgiReport/B987/download/?id=${deviceId}&de=${de}&ate=${ate}&top=${top}`;

    return this.http.get(url, { responseType: 'blob' as 'json' }).pipe(
      map(res => res),
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }
  /**
   * Gets data MCond to graphic.
   */
   getDataMCondGraphic(deviceId: string, de: string, ate: string, skip = 0, top = 0, out: (totalCount: number) => void = null): Observable<MCond[]> {
    const url = `${environment.API_SERVER_URL}/sgiGraphic/b987/${deviceId}?de=${de}&ate=${ate}&skip=${skip}&top=${top}`;
    return this.getEntities<MCond>(() => new MCond(), url, out).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }


  /**
   * Gets reports.
   */
  getDataGraphic(deviceId: string, de: string, ate: string, skip = 0, top = 0, reportType: number = 0, out: (totalCount: number) => void = null): Observable<Report[]> {
    const url = `${environment.API_SERVER_URL}/sgiGraphic/${deviceId}?de=${de}&ate=${ate}&skip=${skip}&top=${top}&reportType=${reportType}`;
    return this.getEntities<Report>(() => new Report(), url, out).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }

  /**
   * Export reports to excel.
   */
  exportReportsToExcel(deviceId: string, de: string, ate: string, top = 0, blocked: boolean = false, reportType: number = 0) {
    this.messageService.isLoadingData = true;
    const url = `${environment.API_SERVER_URL}/sgiReport/download/?id=${deviceId}&de=${de}&ate=${ate}&top=${top}&blocked=${blocked}&reportType=${reportType}`;

    return this.http.get(url, { responseType: 'blob' as 'json' }).pipe(
      map(res => res),
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }


  /**
  * Gets clients.
  */
  getClients(filter: string, statusClient: boolean, isSubClient: boolean, skip = 0, top = 0, out: (totalCount: number) => void = null): Observable<Client[]> {
    const url = `${environment.API_SERVER_URL}/sgiClient/?skip=${skip}&top=${top}&filter=${filter}&statusClient=${statusClient}&isSubClient=${isSubClient}`;
    return this.getEntities<Client>(() => new Client(), url, out).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }

  /**
   * Gets a given client.
   * @param id The client id
   */
  getClient(id: string): Observable<Client> {
    this.messageService.isLoadingData = true;
    return this.http.get<Client>(`${environment.API_SERVER_URL}/sgiClient/${id}`)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false; }),
        map(c => Client.map(c)),
        catchError(this.handleErrorAndContinue('getting client', new Client()))
      );
  }

  /**
   * Gets a given client.
   * @param id The client id
   */
   getClientByDevice(id: string): Observable<Client> {
    this.messageService.isLoadingData = true;
    return this.http.get<Client>(`${environment.API_SERVER_URL}/sgiClient/byDevice/${id}`)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false; }),
        // map(c => Client.map(c)),
        catchError(this.handleErrorAndContinue('getting client by device', new Client()))
      );
  }

  /**
   * Updates or create a client.
   * @param user The client
   */
  saveClient(client: Client, isSubClient: boolean): Observable<Client> {
    const data = JSON.stringify(client);
    let action = null;
    if (client.clientUId) {
      action = this.http.put<Client>(`${environment.API_SERVER_URL}/sgiClient/${client.clientUId}?isSubClient=${isSubClient}`, data, this.httpOptions);
    } else {
      action = this.http.post<Client>(`${environment.API_SERVER_URL}/sgiClient/?isSubClient=${isSubClient}`, data, this.httpOptions);
    }
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  /**
   * Updates a device fields.
   * @param user The device type
   */
  sendChangesDevice(id: string, numeroEnvios: number, tempoTransmissao: number, tipoEnvio: boolean, tensaoMinima: number): Observable<Client> {
    // const data = JSON.stringify(client);
    let action = null;
    const _url = `${environment.API_SERVER_URL}/sgiDashboard/?id=${id}&numeroEnvios=${numeroEnvios}&tempoTransmissao=${tempoTransmissao}&tipoEnvio=${tipoEnvio}&tensaoMinima=${tensaoMinima}`;
    action = this.http.put<Client>(_url, this.httpOptions);
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  sendChangeSerialNumber(id: string, serialNumber: string): Observable<DeviceRegistration> {
    let action = null;
    const _url = `${environment.API_SERVER_URL}/sgiDeviceRegistration/change-serial-number/${id}?serialNumber=${serialNumber}`;
    action = this.http.put<DeviceRegistration>(_url, this.httpOptions);
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  changeFieldsTRM11(id: string, field: string, value: string): Observable<DeviceRegistration> {
    let action = null;
    const _url = `${environment.API_SERVER_URL}/sgiDeviceRegistration/change-fields-trm11/${id}?field=${field}&value=${value}`;
    action = this.http.put<DeviceRegistration>(_url, this.httpOptions);
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  sendChangeModel(id: string, model: string): Observable<DeviceRegistration> {
    let action = null;
    const _url = `${environment.API_SERVER_URL}/sgiDeviceRegistration/change-model/${id}?model=${model}`;
    action = this.http.put<DeviceRegistration>(_url, this.httpOptions);
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  sendChangeNotes(id: string, notes: string): Observable<DeviceRegistration> {
    let action = null;
    const _url = `${environment.API_SERVER_URL}/sgiDeviceRegistration/change-notes/${id}?notes=${notes}`;
    action = this.http.put<DeviceRegistration>(_url, this.httpOptions);
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  cleanPartialbyDevice(id: string, partial: number): Observable<Client> {
    let action = null;
    const _url = `${environment.API_SERVER_URL}/sgiDashboard/clean-partial/${id}?partial=${partial}`;
    action = this.http.put<Client>(_url, this.httpOptions);
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  /**
   * Disable a client, the method don´t delete a client, only disable.
   * @param user The client
   */
  deleteClient(id: string): Observable<string> {
    this.messageService.isLoadingData = true;
    return this.http.delete(`${environment.API_SERVER_URL}/sgiClient/${id}`, this.httpOptions).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError(null))
    );
  }

  /**
   * Gets a given dashboard.
   * @param id The dashboard of device id
   */
  getDashboard(id: string, date: string = null, seqNumber: number = 0, navigation: string = null, seqNumberb: number = 0, project: string = null): Observable<Dashboard> {
    this.messageService.isLoadingData = true;
    return this.http.get<Dashboard>(`${environment.API_SERVER_URL}/sgiDashboard/${id}/?date=${date}&seqNumber=${seqNumber}&navigation=${navigation}&seqNumberb=${seqNumberb}&project=${project}`)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false; }),
        catchError(this.handleErrorAndContinue('getting dashboard', new Dashboard()))
      );
  }

  /**
  * Gets all projects.
  */
  getProjects(skip = 0, top = 0, filter = '', out: (totalCount: number) => void = null): Observable<Project[]> {
    const url = environment.API_SERVER_URL + '/sgiProject/?skip=' + skip + '&top=' + top + '&filter=' + filter;
    return this.getEntities<Project>(() => new Project(), url, out).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }

  /**
  * Gets me projects.
  */
  getMeProjects(): Observable<Project[]> {
    const url = environment.API_SERVER_URL + '/sgiProject/me';
    return this.getEntities<Project>(() => new Project(), url, null).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }

  /**
   * Gets a given project.
   * @param id The project id
   */
  getProject(id: string): Observable<Project> {
    this.messageService.isLoadingData = true;
    return this.http.get<Project>(environment.API_SERVER_URL + '/sgiProject/' + id)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false; }),
        catchError(this.handleErrorAndContinue('getting project', new Project()))
      );
  }

  /**
   * Updates or create a project.
   * @param user The project
   */
  saveProject(project: Project): Observable<Project> {
    const data = JSON.stringify(project);
    let action = null;
    if (project.projectUId) {
      action = this.http.put<Project>(environment.API_SERVER_URL + '/sgiProject/' + project.projectUId, data, this.httpOptions);
    } else {
      action = this.http.post<Project>(environment.API_SERVER_URL + '/sgiProject/', data, this.httpOptions);
    }
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  /**
   * Disable a project, the method don´t delete a project, only disable.
   * @param user The project
   */
  deleteProject(id: string): Observable<string> {
    this.messageService.isLoadingData = true;
    return this.http.delete(environment.API_SERVER_URL + '/sgiProject/' + id, this.httpOptions).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError(null))
    );
  }


  /**
  * Gets packages.
  */
  getPackages(skip = 0, top = 0, filter = '', out: (totalCount: number) => void = null): Observable<Package[]> {
    const url = environment.API_SERVER_URL + '/sgiPackage/?skip=' + skip + '&top=' + top + '&filter=' + filter;
    return this.getEntities<Package>(() => new Package(), url, out).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }

  /**
     * Gets a given package.
     * @param id The package id
     */
  getPackage(id: string): Observable<Package> {
    this.messageService.isLoadingData = true;
    return this.http.get<Package>(environment.API_SERVER_URL + '/sgiPackage/' + id)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false; }),
        catchError(this.handleErrorAndContinue('getting package', new Package()))
      );
  }

  /**
   * Updates or create a package.
   * @param user The package
   */
  savePackage(pack: Package): Observable<Package> {
    const data = JSON.stringify(pack);
    let action = null;
    if (pack.packageUId) {
      action = this.http.put<Package>(environment.API_SERVER_URL + '/sgiPackage/' + pack.packageUId, data, this.httpOptions);
    } else {
      action = this.http.post<Package>(environment.API_SERVER_URL + '/sgiPackage/', data, this.httpOptions);
    }
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  /**
   * Disable a package, the method don´t delete a package, only disable.
   * @param user The package
   */
  deletePackage(id: string): Observable<string> {
    this.messageService.isLoadingData = true;
    return this.http.delete(environment.API_SERVER_URL + '/sgiPackage/' + id, this.httpOptions).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError(null))
    );
  }


  /**
  * Gets devices registration.
  */
  getDevicesRegistrations(skip = 0, top = 0, filter = '', out: (totalCount: number) => void = null): Observable<DeviceRegistration[]> {
    const url = environment.API_SERVER_URL + '/sgiDeviceRegistration/?skip=' + skip + '&top=' + top + '&filter=' + filter;
    return this.getEntities<DeviceRegistration>(() => new DeviceRegistration(), url, out).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }

  /**
     * Gets a given device registration.
     * @param id The device registration id
     */
  getDeviceRegistration(id: string): Observable<DeviceRegistration> {
    this.messageService.isLoadingData = true;
    return this.http.get<DeviceRegistration>(environment.API_SERVER_URL + '/sgiDeviceRegistration/' + id)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false; }),
        catchError(this.handleErrorAndContinue('getting device', new DeviceRegistration()))
      );
  }

  /**
     * Gets a given device registration.
     * @param id The device registration id
     */
   getDeviceRegistrationByDeviceID(id: string): Observable<DeviceRegistration> {
    this.messageService.isLoadingData = true;
    return this.http.get<DeviceRegistration>(environment.API_SERVER_URL + '/sgiDeviceRegistration/byDeviceId/' + id)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false; }),
        catchError(this.handleErrorAndContinue('getting device', new DeviceRegistration()))
      );
  }

  /**
     * Gets a given device registration.
     * @param id The device registration id
     */
  getDeviceLocation(id: string): Observable<DeviceLocation> {
    this.messageService.isLoadingData = true;
    return this.http.get<DeviceLocation>(`${environment.API_SERVER_URL}/sgiDeviceLocation/${id}`)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false; }),
        catchError(this.handleErrorAndContinue('getting device location', new DeviceLocation()))
      );
  }

  /**
   * Updates or create a device registration.
   * @param user The device registration
   */
  saveDeviceRegistration(devReg: DeviceRegistration): Observable<DeviceRegistration> {
    const data = JSON.stringify(devReg);
    let action = null;
    if (devReg.deviceCustomUId) {
      action = this.http.put<DeviceRegistration>(environment.API_SERVER_URL + '/sgiDeviceRegistration/' + devReg.deviceCustomUId, data, this.httpOptions);
    } else {
      action = this.http.post<DeviceRegistration>(environment.API_SERVER_URL + '/sgiDeviceRegistration/', data, this.httpOptions);
    }
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  /**
   * Disable a device registration, the method don´t delete a device registration, only disable.
   * @param user The device registration
   */
  deleteDeviceRegistration(id: string): Observable<string> {
    this.messageService.isLoadingData = true;
    return this.http.delete(environment.API_SERVER_URL + '/sgiDeviceRegistration/' + id, this.httpOptions).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError(null))
    );
  }


  /**
   * Verify or create an user to add on client.
   * @param user The user
   */
  // addUserClient(user: AppUser): Observable<AppUser> {
  //   const data = JSON.stringify(user);

  //   let action = null;
  //   action = this.http.post<AppUser>(`${environment.API_SERVER_URL}/sgiClient/addUserClient/`, data, this.httpOptions);
  //   this.messageService.isLoadingData = true;
  //   return action.pipe(
  //     tap(() => { this.messageService.isLoadingData = false; }),
  //     catchError(this.handleError())
  //   );
  // }

  /**
 * Verify or create an user to add on client.
 * @param user The user
 */
  addUserClient(clientUId: string, user: AppUser): Observable<AppUser> {
    const data = JSON.stringify(user);
    let action = null;
    action = this.http.post<AppUser>(`${environment.API_SERVER_URL}/sgiClient/addUserClient/${clientUId}`, data, this.httpOptions);
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  getStates(): State[] {
    return [{
      id: 1,
      sigla: 'AC',
      name: 'Acre'
    },
    {
      id: 2,
      sigla: 'AL',
      name: 'Alagoas'
    },
    {
      id: 3,
      sigla: 'AM',
      name: 'Amazonas'
    },
    {
      id: 4,
      sigla: 'AP',
      name: 'Amapá'
    },
    {
      id: 5,
      sigla: 'BA',
      name: 'Bahia'
    },
    {
      id: 6,
      sigla: 'CE',
      name: 'Ceará'
    },
    {
      id: 7,
      sigla: 'DF',
      name: 'Distrito Federal'
    },
    {
      id: 8,
      sigla: 'ES',
      name: 'Espírito Santo'
    },
    {
      id: 9,
      sigla: 'GO',
      name: 'Goiás'
    },
    {
      id: 10,
      sigla: 'MA',
      name: 'Maranhão'
    },
    {
      id: 11,
      sigla: 'MG',
      name: 'Minas Gerais'
    },
    {
      id: 12,
      sigla: 'MS',
      name: 'Mato Grosso do Sul'
    },
    {
      id: 13,
      sigla: 'MT',
      name: 'Mato Grosso'
    },
    {
      id: 14,
      sigla: 'PA',
      name: 'Pará'
    },
    {
      id: 15,
      sigla: 'PB',
      name: 'Paraíba'
    },
    {
      id: 16,
      sigla: 'PE',
      name: 'Pernambuco'
    },
    {
      id: 17,
      sigla: 'PI',
      name: 'Piauí'
    },
    {
      id: 18,
      sigla: 'PR',
      name: 'Paraná'
    },
    {
      id: 19,
      sigla: 'RJ',
      name: 'Rio de Janeiro'
    },
    {
      id: 20,
      sigla: 'RN',
      name: 'Rio Grande do Norte'
    },
    {
      id: 21,
      sigla: 'RO',
      name: 'Rondônia'
    },
    {
      id: 22,
      sigla: 'RR',
      name: 'Roraima'
    },
    {
      id: 23,
      sigla: 'RS',
      name: 'Rio Grande do Sul'
    },
    {
      id: 24,
      sigla: 'SC',
      name: 'Santa Catarina'
    },
    {
      id: 25,
      sigla: 'SE',
      name: 'Sergipe'
    },
    {
      id: 26,
      sigla: 'SP',
      name: 'São Paulo'
    },
    {
      id: 27,
      sigla: 'TO',
      name: 'Tocantins'
    }];
  }

  getJsonCEP(cep: string): Observable<Cep> {
    this.messageService.isLoadingData = true;
    return this.http.get<Cep>(`https://viacep.com.br/ws/${cep}/json/`)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false; }),
        map(u => Object.assign(new Cep(), u)),
        catchError(this.handleErrorAndContinue('getting cep', new Cep()))
      );
  }

  /**
  * Remove an user client.
  * @param user The user
  */
  removeUserClient(clientUser: ClientUser): Observable<ClientUser> {
    const data = JSON.stringify(clientUser);
    let action = null;
    action = this.http.post<ClientUser>(`${environment.API_SERVER_URL}/sgiClient/removeUserClient/`, data, this.httpOptions);
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  /**
  * Gets outgoings.
  */
  getOutgoings(skip = 0, top = 0, filter = '', month = 0, year = 0, out: (totalCount: number) => void = null): Observable<Outgoing[]> {
    const url = `${environment.API_SERVER_URL}/sgiOutgoing/?skip=${skip}&top=${top}&filter=${filter}&month=${month}&year=${year}`;
    return this.getEntities<Outgoing>(() => new Outgoing(), url, out).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError([]))
    );
  }

  /**
     * Gets a given outgoing.
     * @param id The outgoing id
     */
  getOutgoing(id: string): Observable<Outgoing> {
    this.messageService.isLoadingData = true;
    let _url = `${environment.API_SERVER_URL}/sgiOutgoing/${id}`;
    return this.http.get<Outgoing>(_url)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false; }),
        catchError(this.handleErrorAndContinue('getting outgoing', new Outgoing()))
      );
  }

  /**
     * Gets a given outgoing.
     * @param id The outgoing id
     */
    getOutgoingShow(id: string): Observable<OutgoingViewModel> {
      this.messageService.isLoadingData = true;
      let _url = `${environment.API_SERVER_URL}/sgiOutgoing/show/${id}`;
      return this.http.get<OutgoingViewModel>(_url)
        .pipe(
          tap(() => { this.messageService.isLoadingData = false; }),
          catchError(this.handleErrorAndContinue('getting outgoing show', new OutgoingViewModel()))
        );
    }

  /**
   * Updates or create a package.
   * @param user The package
   */
  saveOutgoing(outgoing: Outgoing): Observable<Outgoing> {
    const data = JSON.stringify(outgoing);
    let action = null;
    if (outgoing.outgoingUId) {
      action = this.http.put<Outgoing>(`${environment.API_SERVER_URL}/sgiOutgoing/${outgoing.outgoingUId}`, data, this.httpOptions);
    } else {
      action = this.http.post<Outgoing>(`${environment.API_SERVER_URL}/sgiOutgoing/`, data, this.httpOptions);
    }
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  /**
   * Disable a outgoing, the method don´t delete a outgoing, only disable.
   * @param user The outgoing
   */
  deleteOutgoing(id: string): Observable<string> {
    this.messageService.isLoadingData = true;
    return this.http.delete(`${environment.API_SERVER_URL}/sgiOutgoing/${id}`, this.httpOptions).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError(null))
    );
  }

  // updateSigfoxDevices() {
  //   console.log('updateSigfoxDevices');
  //   this.messageService.isLoadingData = true;
  //   return this.http.head(`${environment.IDENTITY_SERVER_URL}/sgisigfox/downloaddevicessigfox`);
  // }

  // updateSigfoxMessages() {
  //   console.log('updateSigfoxMessages');
  //   this.messageService.isLoadingData = true;
  //   return this.http.head(`${environment.IDENTITY_SERVER_URL}/sgisigfox/downloadmessagessigfox`);
  // }

  // updateSigfoxMessages(): Observable<string> {
  //   this.messageService.isLoadingData = true;
  //   return this.http.get(`${environment.IDENTITY_SERVER_URL}/sgisigfox/downloaddevicessigfox`, this.httpOptions).pipe(
  //     tap(() => { this.messageService.isLoadingData = false; }),
  //     catchError(this.handleError(null))
  //   );
  // }

}
