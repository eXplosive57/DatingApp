import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-memebr-card',
  templateUrl: './memebr-card.component.html',
  styleUrls: ['./memebr-card.component.css']
})
export class MemebrCardComponent implements OnInit {
  @Input() user: User;
  constructor() { }

  ngOnInit() {
  }

}
