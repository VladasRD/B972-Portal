import { Component, Inject, OnInit } from '@angular/core';
import { MAT_BOTTOM_SHEET_DATA } from '@angular/material';

@Component({
  selector: 'app-servicedesk-history-bottomsheet',
  templateUrl: './servicedesk-history-bottomsheet.component.html',
  styleUrls: ['./servicedesk-history-bottomsheet.component.css']
})
export class ServicedeskHistoryBottomsheetComponent implements OnInit {

  records: any;
  
  constructor(@Inject(MAT_BOTTOM_SHEET_DATA) public data: any) {
    this.records = data.records;
  }

  ngOnInit() {
  }
  
}
