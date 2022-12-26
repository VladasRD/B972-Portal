import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { MessageService } from '../../common/message.service';
import { GenericYesNoDialogComponent } from '../../common/generic-yes-no-dialog/generic-yes-no-dialog.component';
import { String } from 'typescript-string-operations';
import { FormUtil } from '../../common/form-util';
import { Project } from '../project';

@Component({
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.css']
})
export class ProjectDetailComponent implements OnInit {
  private _projectUId: string;
  project: Project;
  form: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private sgiService: SmartGeoIotService,
    private router: Router,
    private messageService: MessageService,
    public dialog: MatDialog  ) {
    this.project = new Project();
  }

  displayedColumns: string[] = [ 'name', 'description' ];

  ngOnInit() {
    this.form = new FormGroup({
      'code': new FormControl(null, [Validators.required, Validators.maxLength(200)]),
      'name': new FormControl(null, [Validators.required, Validators.maxLength(50)]),
      'description': new FormControl(null, [Validators.required, Validators.maxLength(400)])
    });

    this.getProject();
  }

  get pageTitle(): string {
    if (this.project == null) {
      return '';
    }

    if (this.isNewProject) {
      return 'Novo projeto';
    }
    return String.Format('Projeto {0}', this.project.name);
  }

  get isNewProject(): boolean {
    return this._projectUId === 'new';
  }

  private getProject(): void {
    this._projectUId = this.route.snapshot.paramMap.get('id');
    if (this.isNewProject) {
      return;
    }

    this.sgiService.getProject(this._projectUId).subscribe(project => {
      this.project = Object.assign(new Project(), project);
      this.form.patchValue(this.project);
      this.form.updateValueAndValidity();
    });
  }

  saveProject() {

    if (this.form.invalid) {
      return;
    }

    // updates the model
    FormUtil.updateModel(this.form, this.project);

    // updates the model
    this.project.code = this.form.get('code').value;
    this.project.name = this.form.get('name').value;
    this.project.description = this.form.get('description').value;

    this.sgiService.saveProject(this.project)
      .subscribe(() => {
        this.router.navigate(['./radiodados/projetos']);
        this.messageService.add('Projeto salvo.');
      },
        err => {
          this.messageService.addError(err.message + ' (salvando projeto)');
        });
  }

  private deleteProject() {
    this.sgiService.deleteProject(this.project.projectUId)
      .subscribe(() => {
        this.messageService.add('Projeto removido.');
        this.router.navigate(['./radiodados/projetos']);
      },
        err => {
          this.messageService.addError(err.message + ' (removendo projeto)');
        });
  }

  openConfirmDeleteDialog(): void {
    const dialogRef = this.dialog.open(GenericYesNoDialogComponent, {
      width: '80%',
      data: { title: 'Remover projeto', message: 'Tem certeza que deseja remover esse projeto?', isWarn: true }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.deleteProject();
      }
    });
  }

}
