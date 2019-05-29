import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RouterModule } from '@angular/router';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: 'bank-account',
            loadChildren: './bank-account/bank-account.module#JhipsterNetSampleApplicationBankAccountModule'
      },
      {
        path: 'label',
          loadChildren: './label/label.module#JhipsterNetSampleApplicationLabelModule'
      },
      {
        path: 'operation',
          loadChildren: './operation/operation.module#JhipsterNetSampleApplicationOperationModule'
      }
      /* jhipster-needle-add-entity-route - JHipster will add entity modules routes here */
    ])
  ],
  declarations: [],
  entryComponents: [],
  providers: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class JhipsterNetSampleApplicationEntityModule {}
