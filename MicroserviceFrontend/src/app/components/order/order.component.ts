import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTable } from '@angular/material/table';
import { RequestState } from 'src/app/model/request-state.model';
import { Order } from '../../model/order.model';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})
export class OrderComponent implements OnInit {
  displayedColumns: string[] = ['#', 'name', 'price', 'description', 'quantity', 'category'];
  state = RequestState
  loadState = this.state.success
  constructor(private orderService: OrderService) { }

  orders: Order | null = null
  
  ngOnInit(): void {
    this.loadState = this.state.loading
    this.orderService.getOrdersOfUser()
      .subscribe({
        next: result => {
          console.log('orders:',result)
          this.orders = result
          this.loadState = this.state.success
        },
        error: error => {
          this.loadState = this.state.error
        }
    })
  }

}
