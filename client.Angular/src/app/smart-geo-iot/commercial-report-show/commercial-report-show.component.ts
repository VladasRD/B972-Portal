import { GenericYesNoDialogComponent } from './../../common/generic-yes-no-dialog/generic-yes-no-dialog.component';
import { FormUtil } from './../../common/form-util';
import { Utils } from './../utils';
import { MatDialog } from '@angular/material';
import { MessageService } from './../../common/message.service';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { Outgoing, OutgoingClient } from './../outgoing';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-commercial-report-show',
  templateUrl: './commercial-report-show.component.html',
  styleUrls: ['./commercial-report-show.component.css']
})
export class CommercialReportShowComponent implements OnInit {
  private _outgoingUId: string;
  outgoing: Outgoing;
  clients = [];

  constructor(
    private route: ActivatedRoute,
    private sgiService: SmartGeoIotService,
    private router: Router,
    private messageService: MessageService,
    public dialog: MatDialog,
    private formBuilder: FormBuilder
  ) {
    this.outgoing = new Outgoing();
  }

  ngOnInit() {
    this.getOutgoing();
  }

  get pageTitle(): string {
    return `Portal RadioDados`;
  }

  get pageSubTitle(): string {
    return `Relatório de Fluxo de Clientes`;
  }

  get pageData(): string {
    if (this.outgoing != null) {
      return `${Utils.enumMonths[this.outgoing.month]} de ${this.outgoing.year}`;
    }
    return '00/00/0000';
  }

  private getOutgoing(): void {
    this._outgoingUId = this.route.snapshot.paramMap.get('id');

    this.sgiService.getOutgoingShow(this._outgoingUId).subscribe(r => {
      this.outgoing = Object.assign(new Outgoing(), r.outgoing);
      
      r.clients.forEach(f => {
        this.clients.push(f);
      });
    });
  }

  printReport() {
    window.print();
  }

}
