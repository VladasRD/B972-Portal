import { FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { String } from 'typescript-string-operations';

@Component({
  selector: 'app-graphic-detail',
  templateUrl: './graphic-detail.component.html',
  styleUrls: ['./graphic-detail.component.css']
})
export class GraphicDetailComponent implements OnInit {

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
    return String.Format('Gráfico do dispositivo {0} (Hidroponia)', this._deviceId);
  }

}
