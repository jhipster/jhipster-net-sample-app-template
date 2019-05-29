import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RouterModule } from '@angular/router';
import { JhiLanguageService } from 'ng-jhipster';
import { JhiLanguageHelper } from 'app/core';

import { JhipsterNetSampleApplicationSharedModule } from 'app/shared';
import {
  OperationComponent,
  OperationDetailComponent,
  OperationUpdateComponent,
  OperationDeletePopupComponent,
  OperationDeleteDialogComponent,
  operationRoute,
  operationPopupRoute
} from './';

const ENTITY_STATES = [...operationRoute, ...operationPopupRoute];

@NgModule({
    imports: [JhipsterNetSampleApplicationSharedModule, RouterModule.forChild(ENTITY_STATES)],
  declarations: [
    OperationComponent,
    OperationDetailComponent,
    OperationUpdateComponent,
    OperationDeleteDialogComponent,
    OperationDeletePopupComponent
  ],
  entryComponents: [OperationComponent, OperationUpdateComponent, OperationDeleteDialogComponent, OperationDeletePopupComponent],
  providers: [{ provide: JhiLanguageService, useClass: JhiLanguageService }],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class JhipsterNetSampleApplicationOperationModule {
  constructor(private languageService: JhiLanguageService, private languageHelper: JhiLanguageHelper) {
    this.languageHelper.language.subscribe((languageKey: string) => {
      if (languageKey !== undefined) {
        this.languageService.changeLanguage(languageKey);
      }
    });
  }
}
