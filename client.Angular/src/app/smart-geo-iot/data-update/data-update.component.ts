import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { MessageService } from '../../common/message.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-data-update',
  templateUrl: './data-update.component.html',
  styleUrls: ['./data-update.component.css']
})
export class DataUpdateComponent implements OnInit {
  form: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private sgiService: SmartGeoIotService,
    private router: Router,
    private messageService: MessageService,
    public dialog: MatDialog
  ) {

  }

  ngOnInit() {
    this.form = new FormGroup({
      'deviceFilter': new FormControl(null)
    });

    this.form.get('deviceFilter').valueChanges.subscribe(val => {
      console.log(this.deviceFilter);
    });
  }

  updateDevices() {
    this.messageService.blockUI();
    window.open(`${environment.IDENTITY_SERVER_URL}/sgisigfox/downloaddevicessigfox`, "_blank");
    this.messageService.isLoadingData = false;
  }

  updateMessages() {
    this.messageService.blockUI();
    window.open(`${environment.IDENTITY_SERVER_URL}/sgisigfox/downloadmessagessigfox`, "_blank");
    this.messageService.isLoadingData = false;
  }

  updateMessagesOLD() {
    this.messageService.blockUI();
    window.open(`${environment.IDENTITY_SERVER_URL}/sgisigfox/DownloadOLDMessagesSigfox`, "_blank");
    this.messageService.isLoadingData = false;
  }

  get deviceFilter() {
    return this.form.get('deviceFilter').value;
  }


}
