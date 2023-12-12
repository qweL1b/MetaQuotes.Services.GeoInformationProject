import { renderHomePage } from './homePage.js';
import { renderGeoSearch, initGeoSearchPage } from './geoSearchPage.js';
import { renderCitySearch, initCitySearchPage } from './citySearchPage.js';
import { renderNotFoundPage } from './notFoundPage.js';

function router() {
    const routes = {
        '/': renderHomePage,
        '/geo-search': renderGeoSearch,
        '/city-search': renderCitySearch // Добавление нового маршрута
    };

    const path = window.location.hash.substring(1);
    const renderFunction = routes[path] || renderNotFoundPage;

    document.getElementById('app').innerHTML = renderFunction();

    if (path === '/geo-search') {
        initGeoSearchPage();
    } else if (path === '/city-search') {
        initCitySearchPage();
    }
}

window.addEventListener('hashchange', router);
window.addEventListener('load', router);
