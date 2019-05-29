import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import {
    JhipsterNetSampleApplicationSharedLibsModule,
    JhipsterNetSampleApplicationSharedCommonModule,
  JhiLoginModalComponent,
  HasAnyAuthorityDirective
} from './';

@NgModule({
    imports: [JhipsterNetSampleApplicationSharedLibsModule, JhipsterNetSampleApplicationSharedCommonModule],
  declarations: [JhiLoginModalComponent, HasAnyAuthorityDirective],
  entryComponents: [JhiLoginModalComponent],
    exports: [JhipsterNetSampleApplicationSharedCommonModule, JhiLoginModalComponent, HasAnyAuthorityDirective],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class JhipsterNetSampleApplicationSharedModule {
  static forRoot() {
    return {
        ngModule: JhipsterNetSampleApplicationSharedModule
    };
  }
}
