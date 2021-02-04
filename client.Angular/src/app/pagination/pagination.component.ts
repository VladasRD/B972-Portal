import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.css']
})
export class PaginationComponent implements OnInit {

  @Input() pageSize = 12;
  @Input() totalCount = -1;
  @Input() skip = 0;
  @Input() resultCount = 0;
  @Output() pageChanged = new EventEmitter<Number>();

  constructor() { }

  ngOnInit() {
  }

  get isFirstPage() {
    return this.skip <= 0;
  }

  get isLastPage() {
    if (this.totalCount < 0) {
      return this.resultCount === 0 && this.skip > 0;
    }
    return this.skip + this.pageSize >= this.totalCount;
  }

  get currentPage() {
    return (this.skip / this.pageSize) + 1;
  }

  get totalPages() {
    if (this.totalCount < 0) {
      return 1;
    }
    return Math.ceil(this.totalCount / this.pageSize);
  }

  get hasTotalPages() {
    return this.totalCount >= 0;
  }

   /**
   * Go to the next page.
   */
  goToNextPage(): void {
    this.skip = this.skip + this.pageSize;
    this.pageChanged.emit(this.skip);
  }

  /**
   * Go to the previous page.
   */
  goToPreviousPage(): void {
    if (this.skip <= 0) {
      return;
    }
    this.skip = this.skip - this.pageSize;
    this.pageChanged.emit(this.skip);
  }

  goToFirstPage(): void {
    this.skip = 0;
    this.pageChanged.emit(this.skip);
  }

  goToLastPage(): void {
    this.skip = (this.totalPages - 1) * this.pageSize;
    this.pageChanged.emit(this.skip);
  }




}
