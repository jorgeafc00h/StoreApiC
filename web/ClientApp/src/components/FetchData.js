import React, { Component } from 'react';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor (props) {
    super(props);
    this.state = { forecasts: [], loading: true };

      fetch('https://storeapicoding.azurewebsites.net/api/products')
      .then(response => response.json())
      .then(data => {
        this.setState({ products: data, loading: false });
      });
  }

  static renderForecastsTable (products) {
    return (
      <table className='table table-striped'>
        <thead>
          <tr>
            
            <th>Name</th>
            <th>Description</th>
            <th>Price</th>
                    <th>Stock</th>
                    <th>Max Stock Threshold</th>
                    <th>Min Stock Threshold</th>
          </tr>
        </thead>
        <tbody>
          {products.map(product =>
              <tr key={product.id}>
                  <td>{product.name}</td>
                  <td>{product.description}</td>
                  <td>{product.price}</td>
                  <td>{product.availableStock}</td>
                  <td>{product.maxStockThreshold}</td>
                  <td>{product.restockThreshold}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render () {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : FetchData.renderForecastsTable(this.state.products);

    return (
      <div>
        <h1>Products</h1>
        <p>All available products.</p>
        {contents}
      </div>
    );
  }
}
