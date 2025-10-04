import { Component } from '@angular/core';
import { CardComponent } from '../../../../shared/components/card/card.component';

@Component({
  selector: 'app-teams',
  standalone: true,
  imports: [CardComponent],
  templateUrl: './teams.html',
  styleUrls: ['./teams.css']
})
export class TeamsPage {

}
