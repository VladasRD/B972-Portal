import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { LogService } from '../log.service';
import { AppLog } from '../appLog';


@Component({
  selector: 'app-log-detail',
  templateUrl: './log-detail.component.html',
  styleUrls: ['./log-detail.component.css']
})
export class LogDetailComponent implements OnInit {

  log: AppLog;

  constructor(private route: ActivatedRoute, private logService: LogService) { }

  ngOnInit() {
    this.getLog();
  }

  private getLog(): void {
    const id = this.route.snapshot.paramMap.get('id');

    this.logService.getLog(id).subscribe(log => {
      this.log = log;
    });
  }

}
