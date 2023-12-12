export function renderGeoSearch() {
    return `
        <div class="search-container">
            <h1>Поиск Гео-Информации</h1>
            <input type="text" id="ipInput" class="search-input" placeholder="Введите IP адрес">
            <button id="searchButton" class="search-button">Искать</button>
            <div id="results" class="search-results"></div>
        </div>
    `;
}

export function initGeoSearchPage() {
    document.getElementById('searchButton').addEventListener('click', function () {
        var ip = document.getElementById('ipInput').value;
        fetch('/ip/location?ip=' + ip)
            .then(function (response) {
                return response.json();
            })
            .then(function (data) {
                var html = '<table class="results-table">';
                html += '<tr><th>Country</th><th>Region</th><th>Postal</th><th>City</th><th>Organization</th><th>Latitude</th><th>Longitude</th></tr>';
                html += `<tr>
                        <td>${data.country}</td>
                        <td>${data.region}</td>
                        <td>${data.postal}</td>
                        <td>${data.city}</td>
                        <td>${data.organization}</td>
                        <td>${data.latitude}</td>
                        <td>${data.longitude}</td>
                    </tr>`;
                html += '</table>';
                document.getElementById('results').innerHTML = html;
            })
            .catch(function (error) {
                document.getElementById('results').textContent = 'Ошибка: ' + error;
            });
    });
}
