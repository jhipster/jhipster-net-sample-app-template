import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

import { NhipsterSampleApplicationBankAccountModule } from './bank-account/bank-account.module';
import { NhipsterSampleApplicationLabelModule } from './label/label.module';
import { NhipsterSampleApplicationOperationModule } from './operation/operation.module';
/* jhipster-needle-add-entity-module-import - JHipster will add entity modules imports here */

@NgModule({
  // prettier-ignore
  imports: [
        NhipsterSampleApplicationBankAccountModule,
        NhipsterSampleApplicationLabelModule,
        NhipsterSampleApplicationOperationModule,
        /* jhipster-needle-add-entity-module - JHipster will add entity modules here */
    ],
  declarations: [],
  entryComponents: [],
  providers: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class NhipsterSampleApplicationEntityModule {}
