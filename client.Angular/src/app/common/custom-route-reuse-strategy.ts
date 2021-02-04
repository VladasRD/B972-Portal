import { RouteReuseStrategy, DetachedRouteHandle, ActivatedRouteSnapshot } from '@angular/router';
import { IRefreshableComponent } from './i-refreshable-component';

export class CustomRouteReuseStrategy implements RouteReuseStrategy {

  storedRouteHandles = new Map<string, DetachedRouteHandle>();

  // Decides if the route should be stored
  shouldDetach(route: ActivatedRouteSnapshot): boolean {
    return route.data.shouldReuse || false;
  }

  // Store the information for the route we're destructing
  store(route: ActivatedRouteSnapshot, handle: DetachedRouteHandle): void {
     this.storedRouteHandles.set(route.routeConfig.path, handle);
  }

  // Return true if we have a stored route object for the next route
  shouldAttach(route: ActivatedRouteSnapshot): boolean {
     return this.storedRouteHandles.has(route.routeConfig.path);
  }

  // If we returned true in shouldAttach(), now return the actual route data for restoration
  retrieve(route: ActivatedRouteSnapshot): DetachedRouteHandle {
     // return this.storedRouteHandles.get(route.routeConfig.path);

     if (!route.routeConfig) {
      return null;
    }

    // tries to get the snapshot
    const snap = this.storedRouteHandles.get(route.routeConfig.path);
    if (snap == null) {
      return null;
    }

    // if the snapshot is from i Refreshable component, refresh it
    const instance = snap['componentRef']['instance'];
    if (instance != null && (<IRefreshableComponent>instance).updateResults) {
      const comp = instance as IRefreshableComponent;
      comp.updateResults();
    }
    return snap;
  }

  // Reuse the route if we're going to and from the same route
  shouldReuseRoute(future: ActivatedRouteSnapshot, curr: ActivatedRouteSnapshot): boolean {
     return future.routeConfig === curr.routeConfig;
  }

}
