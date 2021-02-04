import { FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { String } from 'typescript-string-operations';

@Component({
  selector: 'app-graphic-aguamon-detail',
  templateUrl: './graphic-aguamon-detail.component.html',
  styleUrls: ['./graphic-aguamon-detail.component.css']
})
export class GraphicAguamonDetailComponent implements OnInit {

  private _deviceId: string;
  form: FormGroup;

  constructor(private route: ActivatedRoute) {
    this._deviceId = this.route.snapshot.paramMap.get('id');
  }

  ngOnInit() {
    this.form = new FormGroup({
    });
  }

  get pageTitle(): string {
    return String.Format('Gráfico do dispositivo {0} (Aguamon)', this._deviceId);
  }

}
