import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTable } from '@angular/material/table';
import { Order } from '../model/order.model';
import { OrderService } from '../services/order.service';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})
export class OrderComponent implements OnInit {
  displayedColumns: string[] = ['#', 'name', 'price', 'description', 'quantity', 'category'];
  
  constructor(private orderService: OrderService) { }

  @ViewChild(MatTable) myTable: MatTable<any> | undefined;
  orders: Order | null = null
  
  ngOnInit(): void {
    this.orderService.getOrdersOfUser()
      .subscribe(result => {
        console.log('orders:',result)
        this.orders = result
        this.myTable?.renderRows()
      })
  }

}
