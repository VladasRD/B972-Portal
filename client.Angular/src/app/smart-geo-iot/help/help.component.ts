import { Component, OnInit } from '@angular/core';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-help',
  templateUrl: './help.component.html',
  styleUrls: ['./help.component.css']
})
export class HelpComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  get getUrlManualCliente() {
    return environment.CLIENT_URL + '/assets/docs/manual-cliente.pdf';
  }

  get getUrlManualAdministrador() {
    return environment.CLIENT_URL + '/assets/docs/manual-administrador.pdf';
  }

}
