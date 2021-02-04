import { Cep } from './../address';
import { Utils } from './../utils';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, Validators, FormArray, FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { Client, ClientDevice, ClientUser, ClientBilling, BillingType } from '../client';
import { MessageService } from '../../common/message.service';
import { GenericYesNoDialogComponent } from '../../common/generic-yes-no-dialog/generic-yes-no-dialog.component';
import { UserPickerDialogComponent } from '../../security/user-picker-dialog/user-picker-dialog.component';
import { String } from 'typescript-string-operations';
import { DocumentType } from '../documentType';
import { FormUtil } from '../../common/form-util';

@Component({
  selector: 'app-client-detail',
  templateUrl: './client-detail.component.html',
  styleUrls: ['./client-detail.component.css']
})
export class ClientDetailComponent implements OnInit {
  private _clientUId: string;
  client: Client;
  form: FormGroup;
  documentTypes: DocumentType[] = [];
  listTypes: any[] = [];

  emailNotification: boolean;
  smsNotification: boolean;
  whatsAppNotification: boolean;
  pushNotification: boolean;

  constructor(
    private route: ActivatedRoute,
    private sgiService: SmartGeoIotService,
    private router: Router,
    private messageService: MessageService,
    public dialog: MatDialog,
    private formBuilder: FormBuilder
  ) {
    this.client = new Client();
    this.client.devices = [];
    this.client.users = [];
    this.client.billings = [];
  }

  cellphoneMask = ['(', /[1-9]/, /\d/, ')', ' ', /\d/, /\d/, /\d/, /\d/, /\d/, '-', /\d/, /\d/, /\d/, /\d/];
  commercialphoneMask = ['(', /[1-9]/, /\d/, ')', ' ', /\d/, /\d/, /\d/, /\d/, '-', /\d/, /\d/, /\d/, /\d/];
  documentMask = '00.000.000/0000-00';
  displayedColumns: string[] = [ 'date', 'paymentDueDate', 'paymentDate', 'status', 'sended', 'pdf' ];

  ngOnInit() {
    this.form = new FormGroup({
      'documentType': new FormControl(null, Validators.required),
      'name': new FormControl(null, [Validators.required, Validators.maxLength(500)]),
      'document': new FormControl(null, Validators.required),
      'postalCode': new FormControl(null, [Validators.required, Validators.maxLength(8)]),
      'email': new FormControl(null, [Validators.required, Validators.maxLength(255)]),
      'phone': new FormControl(''),
      'address': new FormControl({value: '', disabled: true}),
      'addressNumber': new FormControl(''),
      'neighborhood': new FormControl({value: '', disabled: true}),
      'city': new FormControl({value: '', disabled: true}),
      'state': new FormControl({value: '', disabled: true}),
      'active': new FormControl(null),
      'deviceToAdd': new FormControl(),
      'startBilling': new FormControl(null),
      'dueDay': new FormControl(''),
      'item': new FormControl(),
      'type': new FormControl(''),
      'value': new FormControl(''),
      'birth': new FormControl(''),
      'cpf': new FormControl('')
    });

    this.buildDocumentTypeList();
    this.typeBillingList();
    this.getClient();
  }

  handleDocumentTypeChange(documentType) {
    this.documentMask = documentType.value === 1 ? '00.000.000/0000-00' : '000.000.000-00';
    this.form.controls.document.setValue('');
  }

  get postalCode() {
    return this.form.get('postalCode').value;
  }

  changeZipCode() {
    if (this.postalCode.length === 8) {
      this.sgiService.getJsonCEP(this.postalCode).subscribe(cep => {
        if (!cep) {
          return;
        }
        this.buildAddressByCep(cep);
      });
    }
  }

  buildAddressByCep(cep: Cep) {
    if (cep !== null) {
      this.form.get('address').setValue(cep.logradouro);
      this.form.get('neighborhood').setValue(cep.bairro);
      const states = this.sgiService.getStates();
      this.form.get('state').setValue(states.filter(f => f.sigla === cep.uf)[0].name);
      this.form.get('city').setValue(cep.localidade);
    }
  }

  get pageTitle(): string {
    if (this.client == null) {
      return '';
    }

    if (this.isNewClient) {
      return 'Novo cliente';
    }
    return String.Format('Cliente {0}', this.client.name);
  }

  get isNewClient(): boolean {
    return this._clientUId === 'new';
  }

  private getClient(): void {
    this._clientUId = this.route.snapshot.paramMap.get('id');
    if (this.isNewClient) {
      return;
    }

    this.sgiService.getClient(this._clientUId).subscribe(client => {
      this.client = Object.assign(new Client(), client);

      this.emailNotification = this.client.emailNotification;
      this.smsNotification = this.client.smsNotification;
      this.whatsAppNotification = this.client.whatsAppNotification;
      this.pushNotification = this.client.pushNotification;
      this.documentMask = this.client.documentType === 1 ? '00.000.000/0000-00' : '000.000.000-00';

      this.form.patchValue(this.client);
      this.form.updateValueAndValidity();
    });
  }

  private buildDocumentTypeList() {
    this.documentTypes.push(DocumentType.enum[DocumentType.CPF]);
    this.documentTypes.push(DocumentType.enum[DocumentType.CNPJ]);
  }

  addDevice() {
    const device = this.form.get('deviceToAdd').value;
    this.form.get('deviceToAdd').setValue(null);
    if (device == null) {
      return;
    }
    if (this.client.devices.find(c => c.id === device.id)) {
      return;
    }

    const clientDevice = new ClientDevice();
    clientDevice.appDevice = device;
    clientDevice.active = true;
    clientDevice.id = device.id;
    this.client.devices.push(clientDevice);
  }

  changeDeviceStatus(clientDevice: ClientDevice) {
    clientDevice.active = !clientDevice.active;
  }

  changeClientStatus() {
    this.client.active = !this.client.active;
  }

  changeNotification(notification: string) {
    this[notification] = !this[notification];
  }

  get isClientActive(): boolean {
    return this.client.active;
  }

  removeDevice(clientDevice: ClientDevice) {
    this.client.devices.splice(this.client.devices.findIndex(c => c.id === clientDevice.id), 1);
  }

  addUser() {
    const dialogRef = this.dialog.open(UserPickerDialogComponent, UserPickerDialogComponent.config(false));
    dialogRef.afterClosed().subscribe(
      users => {
        if (!users) {
          return;
        }
        users.forEach(u => {
          if (!this.client.users) {
            this.client.users = [];
          }
          const alreadyHasUser = this.client.users.find(s => s.applicationUserId === u.id);
          if (!alreadyHasUser) {
            const user = new ClientUser();
            user.applicationUserId = u.id;
            user.clientUId = this.client.clientUId;
            user.appUser = u;
            this.client.users.push(user);
          }
        });
      });
  }

  removeUser(user: ClientUser) {
    this.client.users.splice(this.client.users.findIndex(s => s.applicationUserId === user.appUser.id), 1);
  }

  saveClient() {

    if (this.form.invalid) {
      return;
    }

    // updates the model
    FormUtil.updateModel(this.form, this.client);

    // updates the model
    this.client.documentType = this.form.get('documentType').value;
    this.client.name = this.form.get('name').value;
    this.client.document = this.form.get('document').value;
    this.client.postalCode = this.form.get('postalCode').value;
    this.client.address = this.form.get('address').value;
    this.client.neighborhood = this.form.get('neighborhood').value;
    this.client.city = this.form.get('city').value;
    this.client.state = this.form.get('state').value;
    this.client.active = this.form.get('active').value;
    this.client.startBilling = this.form.get('startBilling').value;
    this.client.dueDay = this.form.get('dueDay').value;
    this.client.item = this.form.get('item').value;
    this.client.type = this.form.get('type').value;
    this.client.value = this.form.get('value').value;
    this.client.birth = this.form.get('birth').value;
    this.client.cpf = this.form.get('cpf').value;
    this.client.phone = this.form.get('phone').value.replace(/\D/g, '');
    this.client.emailNotification = this.emailNotification;
    this.client.smsNotification = this.smsNotification;
    this.client.whatsAppNotification = this.whatsAppNotification;
    this.client.pushNotification = this.pushNotification;

    // Tratamento de virgula e ponto nos valores
    if (this.client.value !== null) {
      this.client.value = Number(this.client.value.toString().replace(',', '.'));
    }

    this.messageService.blockUI();
    this.sgiService.saveClient(this.client, false)
      .subscribe(() => {
        this.router.navigate(['./sgi/clientes']);
        this.messageService.add('Cliente salvo.');
      },
        err => {
          this.messageService.addError(err.message + ' (salvando cliente)');
        });
  }

  private deleteClient() {
    this.sgiService.deleteClient(this.client.clientUId)
      .subscribe(() => {
        this.messageService.add('Cliente removido.');
        this.router.navigate(['./sgi/clientes']);
      },
        err => {
          this.messageService.addError(err.message + ' (removendo cliente)');
        });
  }

  openConfirmDeleteDialog(): void {
    const dialogRef = this.dialog.open(GenericYesNoDialogComponent, {
      width: '80%',
      data: { title: 'Remover cliente', message: 'Tem certeza que deseja remover esse cliente?', isWarn: true }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.deleteClient();
      }
    });
  }

  private typeBillingList(): void {
    for (let y = 0; y <= BillingType.Anual; y++) {
      this.listTypes.push({ id: y, name: BillingType.enum[y]});
    }
  }

  get billings(): ClientBilling[] {
    if (!this.client || !this.client.billings) {
      return [];
    }
    return this.client.billings;
  }

}
