$(document).ready(function () {
    loadVehicles();

    $('#vehicleForm').on('submit', function (e) {
        e.preventDefault();
    });

    $('#searchForm').on('submit', function (e) {
        e.preventDefault();
        searchVehicle();
    });
});

function loadVehicles() {
    $('#searchContainer').hide();
    $('#formContainer').hide();

    $.ajax({
        url: '/api/vehicles/GetAll',
        method: 'GET',
        success: function (response) {
            let html = '<table class="w-100"><thead><tr><th>Chassis</th><th>Tipo</th><th>Passageiros</th><th>Cor</th><th>Ações</th></tr></thead><tbody>';
            response.vehicles.forEach(v => {
                html += `<tr>
                    <td>${v.chassisId.chassisSeries}-${v.chassisId.chassisNumber}</td>
                    <td>${v.type}</td>
                    <td>${v.numberPassengers}</td>
                    <td>${v.color}</td>
                    <td>
                        <button onclick="showEditForm('${v.chassisId.chassisSeries}', ${v.chassisId.chassisNumber})">Editar Cor</button>
                    </td>
                </tr>`;
            });
            html += '</tbody></table>';
            html += `<div class="vehicle-total">Total de veiculos na frota: ${response.total}</div>`;
            $('#vehicleList').html(html);
        },
        error: function () {
            $('#vehicleList').html('<div class="vehicle-total" style="color: #f44336">Erro ao carregar veiculos</div>');
        }
    });
}

function showCreateForm() {
    $('#formTitle').text('Adicionar Veículo');
    $('#vehicleForm').off('submit').on('submit', createVehicle);
    clearForm();
    $('#searchContainer').hide();
    $('#formContainer').show();
}

function showEditForm(series, number) {
    $('#formTitle').text(`Editar Cor - ${series}-${number}`);
    $('#vehicleForm').off('submit').on('submit', function (e) {
        updateColor(e, series, number);
    });
    $('#chassisSeries').val(series).prop('disabled', true);
    $('#chassisNumber').val(number).prop('disabled', true);
    $('#vehicleType').prop('disabled', true);
    $('#color').val('');
    $('#searchContainer').hide();
    $('#formContainer').show();
}

function showSearchForm() {
    $('#searchSeries').val('');
    $('#searchNumber').val('');
    $('#formContainer').hide();
    $('#searchContainer').show();
}

function hideForm() {
    $('#formContainer').hide();
    $('#vehicleType').prop('disabled', false);
    $('#chassisSeries').prop('disabled', false);
    $('#chassisNumber').prop('disabled', false);
}

function hideSearch() {
    $('#searchContainer').hide();
}

function clearForm() {
    $('#chassisSeries').val('').prop('disabled', false);
    $('#chassisNumber').val('').prop('disabled', false);
    $('#vehicleType').prop('disabled', false).val('Bus');
    $('#color').val('');
}

function createVehicle(e) {
    e.preventDefault();
    const vehicle = {
        chassisId: {
            chassisSeries: $('#chassisSeries').val(),
            chassisNumber: Number($('#chassisNumber').val()),
        },
        type: parseInt($('#vehicleType').val()),
        color: $('#color').val()
    };
    $.ajax({
        url: '/api/vehicles/Create',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(vehicle),
        success: function () {
            hideForm();
            loadVehicles();
        },
        error: function (xhr) {
            showErrorModal('Erro: ' + xhr.responseText);

        }
    });
}

function updateColor(e, series, number) {
    e.preventDefault();

    const color = $('#color').val();
    if (!color) {
        showErrorModal('Erro: ' + 'Erro atualizar cor');
    }

    const updateRequest = {
        chassisSeries: series,
        chassisNumber: number,
        newColor: color
    };

    $.ajax({
        url: '/api/vehicles/Update',
        method: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(updateRequest),
        success: function () {
            hideForm();
            loadVehicles();
        },
        error: function () {
            showErrorModal('Erro: ' + 'Erro atualizar cor');

        }
    });
}

function searchVehicle() {
    const series = $('#searchSeries').val();
    const number = $('#searchNumber').val();

    $.ajax({
        url: `/api/vehicles/Details/${series}/${number}`,
        method: 'GET',
        success: function (vehicle) {
            let html = '<div class="vehicle-total">Resultado da busca: 1 veículo</div>';
            html += '<table class="w-100"><thead><tr><th>Chassis</th><th>Tipo</th><th>Passageiros</th><th>Cor</th><th>Ações</th></tr></thead><tbody>';
            html += `<tr>
                <td>${vehicle.chassisId.chassisSeries}-${vehicle.chassisId.chassisNumber}</td>
                <td>${vehicle.type}</td>
                <td>${vehicle.numberPassengers}</td>
                <td>${vehicle.color}</td>
                <td>
                    <button onclick="showEditForm('${vehicle.chassisId.chassisSeries}', ${vehicle.chassisId.chassisNumber})">Editar Cor</button>
                </td>
            </tr>`;
            html += '</tbody></table>';
            $('#vehicleList').html(html);
        },
        error: function () {
            showErrorModal('Erro: ' + 'veiculo não encontrado');

        }
    });
}
function showErrorModal(message) {
    $('#errorMessage').text(message);
    $('#errorModal').css('display', 'flex').hide().fadeIn();
}

function closeModal() {
    $('#errorModal').fadeOut();
}