import { Package } from './../package';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { MessageService } from '../../common/message.service';
import { GenericYesNoDialogComponent } from '../../common/generic-yes-no-dialog/generic-yes-no-dialog.component';
import { String } from 'typescript-string-operations';
import { FormUtil } from '../../common/form-util';

@Component({
  selector: 'app-package-detail',
  templateUrl: './package-detail.component.html',
  styleUrls: ['./package-detail.component.css']
})
export class PackageDetailComponent implements OnInit {
  private _packageUId: string;
  package: Package;
  form: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private sgiService: SmartGeoIotService,
    private router: Router,
    private messageService: MessageService,
    public dialog: MatDialog,
    private formBuilder: FormBuilder
  ) {
    this.package = new Package();
  }

  displayedColumns: string[] = [ 'name', 'description', 'type' ];

  ngOnInit() {
    this.form = new FormGroup({
      'name': new FormControl(null, [Validators.required, Validators.maxLength(500)]),
      'description': new FormControl(null, [Validators.required]),
      'type': new FormControl(null, [Validators.required]),
    });

    this.getPackage();
  }

  get pageTitle(): string {
    if (this.package == null) {
      return '';
    }

    if (this.isNewPackage) {
      return 'Novo pacote';
    }
    return String.Format('Pacote {0}', this.package.name);
  }

  get isNewPackage(): boolean {
    return this._packageUId === 'new';
  }

  private getPackage(): void {
    this._packageUId = this.route.snapshot.paramMap.get('id');
    if (this.isNewPackage) {
      return;
    }

    this.sgiService.getPackage(this._packageUId).subscribe(project => {
      this.package = Object.assign(new Package(), project);
      this.form.patchValue(this.package);
      this.form.updateValueAndValidity();
    });
  }

  savePackage() {

    if (this.form.invalid) {
      return;
    }

    // updates the model
    FormUtil.updateModel(this.form, this.package);

    // updates the model
    this.package.name = this.form.get('name').value;
    this.package.description = this.form.get('description').value;
    this.package.type = this.form.get('type').value;

    this.sgiService.savePackage(this.package)
      .subscribe(() => {
        this.router.navigate(['./sgi/pacotes']);
        this.messageService.add('Pacote salvo.');
      },
        err => {
          this.messageService.addError(err.message + ' (salvando pacote)');
        });
  }

  private deletePackage() {
    this.sgiService.deletePackage(this.package.packageUId)
      .subscribe(() => {
        this.messageService.add('Pacote removido.');
        this.router.navigate(['./sgi/pacotes']);
      },
        err => {
          this.messageService.addError(err.message + ' (removendo pacote)');
        });
  }

  openConfirmDeleteDialog(): void {
    const dialogRef = this.dialog.open(GenericYesNoDialogComponent, {
      width: '80%',
      data: { title: 'Remover pacote', message: 'Tem certeza que deseja remover esse pacote?', isWarn: true }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.deletePackage();
      }
    });
  }

}
