export function renderCitySearch() {
    return `
        <div class="search-container">
            <h1>Search by city name</h1>
            <input type="text" id="cityInput" class="search-input" placeholder="Enter the name of the city">
            <button id="searchCityButton" class="search-button">Search</button>
            <div id="cityResults" class="search-results"></div>
        </div>
    `;
}

export function initCitySearchPage() {
    document.getElementById('searchCityButton').addEventListener('click', function () {
        var city = document.getElementById('cityInput').value;
        fetch('/city/locations?city=' + city)
            .then(function (response) {
                return response.json();
            })
            .then(function (data) {
                var html = '<table class="results-table">';
                html += '<tr><th>Country</th><th>Region</th><th>Postal</th><th>City</th><th>Organization</th><th>Latitude</th><th>Longitude</th></tr>';
                data.forEach(function (item) {
                    html += `<tr>
                        <td>${item.country}</td>
                        <td>${item.region}</td>
                        <td>${item.postal}</td>
                        <td>${item.city}</td>
                        <td>${item.organization}</td>
                        <td>${item.latitude}</td>
                        <td>${item.longitude}</td>
                    </tr>`;
                });
                html += '</table>';
                document.getElementById('cityResults').innerHTML = html;
            })
            .catch(function (error) {
                document.getElementById('cityResults').textContent = 'Ошибка: ' + error;
            });
    });
}
