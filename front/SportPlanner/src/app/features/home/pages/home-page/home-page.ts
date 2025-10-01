import { Component } from '@angular/core';
import { HeroSection } from '../../../shared/components/hero-section/hero-section';
import { FeaturesSection } from '../../../shared/components/features-section/features-section';
import { PricingSection } from '../../../shared/components/pricing-section/pricing-section';

@Component({
  selector: 'app-home-page',
  imports: [HeroSection, FeaturesSection, PricingSection],
  templateUrl: './home-page.html'
})
export class HomePage {

}
