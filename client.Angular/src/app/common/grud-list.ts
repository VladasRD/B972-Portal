import { Observable, BehaviorSubject } from 'rxjs';
import 'rxjs/add/observable/fromEvent';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/debounceTime';

import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { IRefreshableComponent } from './i-refreshable-component';
import { FormUtil } from './form-util';

// used to detcet screen size changes
const $resizeEvent = Observable.fromEvent(window, 'resize').debounceTime(100);

/**
 * Basic abstract class for a GRUD list with search and paging functionalities.
 */
export abstract class GrudList<T> implements IRefreshableComponent  {

  private lastClientWidth = 0;
  private lastUpdateTime: any;

  protected searchFilter$ = new BehaviorSubject<string>('');

  // for pagination
  protected _pageSize = 16;
  protected _totalCount = -1;
  protected _skip = 0;

  showFilters = true;
  loading = false;
  firstSearch = true;

  results: T[];

  searchPipeConfigured = false;

  /**
   * Should be implemented to call de REST Api and return a observable<T[]>.
   */
  abstract getResults(): Observable<T[]>;

  constructor(searchOnLoad = true) {

    this.results = [];

    if (searchOnLoad) {
      this.configureSearchPipe();
    }

    this.setShowFiltersAccordingWidth();
    $resizeEvent.subscribe(() => { this.setShowFiltersAccordingWidth(); });
  }

  private configureSearchPipe() {
    if (this.searchPipeConfigured) {
      return;
    }
    this.updateResults(
      this.searchFilter$.pipe(
          debounceTime(250),
          distinctUntilChanged(),
          tap(t => {
            this.loading = true;
            this._skip = 0;
          }),
          switchMap(term => this.getResults())
      ));
      this.searchPipeConfigured = true;
  }

  /**
   * Shows or hides filters according screen resolution.
   */
  private setShowFiltersAccordingWidth() {
    const width = document.documentElement.clientWidth;
    if (this.lastClientWidth === width) {
      return;
    }
    this.lastClientWidth = width;
    if  (width > 600) {
      this.showFilters = true;
    } else {
      this.showFilters = false;
    }
  }

  /**
   * Filters the search result using a given term.
   * @param term the term
   */
  filter(term: string): void {
    this.configureSearchPipe();
    this.searchFilter$.next(term);
  }

  /**
   * Filters the search result using a given term.
   * It does nothing at mobile devices.
   * @param term the term
   */
  filterIfNotMobile(term: string): void {
    if (FormUtil.isMobile()) {
      return;
    }
    this.configureSearchPipe();
    this.searchFilter$.next(term);
  }

  pageChanged(skip: number) {
    this._skip = skip;
    this.updateResults();
  }

  newSearch() {
    this._skip = 0;
    this.updateResults();
  }

  /**
   * Update the results with a new the observable collection.
   * @param obsResult the observable collection
   */
  updateResults(obsResult: Observable<T[]> = null): void {

    // chip trick to avoid double gets in short time
    // to procted from route-reuse-strategy issue
    const diff = new Date().getTime() - this.lastUpdateTime;
    if (diff < 100) {
        return;
    }
    this.lastUpdateTime = new Date().getTime();

    if (obsResult == null) {
      obsResult = this.getResults();
    }
    this.loading = true;

    obsResult.subscribe(results => {
      this.results = results;
      this.firstSearch = false;
      this.loading = false;
    });
  }

  get pageSize() {
    return this._pageSize;
  }

  get skip() {
    return this._skip;
  }

  get totalCount() {
    return this._totalCount;
  }

}
