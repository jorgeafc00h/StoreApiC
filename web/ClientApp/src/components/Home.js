import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>Store Api Demo</h1>
        <p>Review Swagger documentation</p>
        <ul>
                <li><a href='http://storeapicoding.azurewebsites.net/swagger/index.html'>Azure Demo</a></li>
          
        </ul>
        
      </div>
    );
  }
}
