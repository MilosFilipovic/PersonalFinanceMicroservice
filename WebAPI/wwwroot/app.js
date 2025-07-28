$(function () {
    const apiBase = window.location.origin;
    let currentPage = 1;
    const pageSize = 10;
    const $container = $('#transactionsContainer');

    // Load TransactionKind dropdown
    $.getJSON(`${apiBase}/api/transactions/kinds`)
        .done(kinds => {
            const $sel = $('#txKind').empty();
            $sel.append('<option value="">All</option>');
            kinds.forEach(k => $sel.append(`<option value="${k}">${k}</option>`));
        });

    $.getJSON(`${apiBase}/api/categories`)
        .done(cats => {
            const $sel = $('#txCategory').empty();
            $sel.append('<option value="">Select Category</option>');
            cats.forEach(cat => $sel.append(
                `<option value="${cat.code}">${cat.name}</option>`
            ));
        })
        .fail(() => console.error('Failed to load categories'));
    
    // Load categories for the categorize form
    $.getJSON(`${apiBase}/api/categories`)
        .done(cats => {
            const $sel = $('#txCategory').empty();
            $sel.append('<option value="">Select Category</option>');
            cats.forEach(c => $sel.append(`<option value="${c.code}">${c.name}</option>`));
        });

    
    function renderTransactions(items) {
        console.log(items)
        const $container = $('#transactionsContainer');
        $container.empty();                        // sad brišeš samo prazno

        const $tpl = $('#transaction-template');    // hvataš skriveni template

        items.forEach(tx => {
            const $card = $tpl.clone()                // klon
                .removeAttr('id')                       // ukloni ID
                .show();                                // i prikaži ga

            // popuni podatke
            $card.find('.tx-id').text(tx.id);
            $card.find('.tx-beneficiary').text(tx['beneficiary-name']);
            $card.find('.tx-date').text(new Date(tx.date).toLocaleDateString());
            const arrow = tx.direction.toLowerCase() === 'credit'
                ? '<span class="text-success">↑</span>'
                : '<span class="text-danger">↓</span>';
            $card.find('.tx-dir').html(arrow);
            $card.find('.tx-amount').text(tx.amount);
            $card.find('.tx-curr').text(tx.currency);
            $card.find('.tx-kind').text(tx.kind);
            $card.find('.tx-desc').text(tx.description);
            $card.find('.tx-mcc').text(tx.mcc || '');
            $card.find('.tx-cat').text(tx['category-code'] || '');

            // ubaci u DOM
            $container.append($card);
        });
    }



    // Update pager controls
    function updatePager(res) {
        currentPage = res.pageNumber;
        const total = Math.ceil(res.totalCount / res.pageSize);
        $('#pageInfo').text(`Page ${currentPage} of ${total}`);
        $('#prevPage').prop('disabled', currentPage <= 1);
        $('#nextPage').prop('disabled', currentPage >= total);
    }

    // Load all transactions
    function loadAllTransactions(page) {
        $.getJSON(`${apiBase}/api/transactions`, { pageNumber: page, pageSize })
            .done(res => {
                renderTransactions(res.items);
                updatePager(res);
            })
            .fail(() => alert('Cannot load transactions'));
    }

    // Load filtered transactions on demand
    function loadFilteredTransactions(page) {
        const start = $('#startDate').val(),
            end = $('#endDate').val(),
            kind = $('#txKind').val();
        if (!start || !end) return alert('Please select both dates');
        const qs = { startDate: start, endDate: end, pageNumber: page, pageSize };
        if (kind) qs.kinds = kind;
        $.getJSON(`${apiBase}/api/transactions/by-date-range`, qs)
            .done(res => {
                renderTransactions(res.items);
                updatePager(res);
            })
            .fail(() => alert('Cannot load filtered transactions'));
    }

    // Bind pagination & filter buttons
    $('#prevPage').click(() =>
        $('#startDate').val() && $('#endDate').val()
            ? loadFilteredTransactions(currentPage - 1)
            : loadAllTransactions(currentPage - 1)
    );
    $('#nextPage').click(() =>
        $('#startDate').val() && $('#endDate').val()
            ? loadFilteredTransactions(currentPage + 1)
            : loadAllTransactions(currentPage + 1)
    );
    $('#filterBtn').click(() => loadFilteredTransactions(1));
    $('#clearBtn').click(() => {
        $('#startDate,#endDate,#txKind').val('');
        loadAllTransactions(1);
    });

    // Bind categorize form
    $('#assignBtn').click(() => {
        const id = $('#txId').val().trim();
        const cat = $('#txCategory').val();
        if (!id) return alert('Enter Transaction ID');
        if (!cat) return alert('Select a Category');

        $.ajax({
            url: `${apiBase}/api/transactions/${id}/categorize`,
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ categoryCode: cat })
        })
            .done(() => {
                alert('Category assigned');
                if ($('#startDate').val() && $('#endDate').val()) {
                    loadFilteredTransactions(currentPage);
                } else {
                    // Nema filtera -> refresuj sve na istoj strani u istom redosledu
                    loadAllTransactions(currentPage);
                    }
            })

            .fail(() => alert('Error assigning category'));
    });


    // Initial load
    loadAllTransactions(1);

    // PFM sidebar click (reloads page)
    $('#pfmBtn').click(e => {
        e.preventDefault();
        $('#sidebar .nav-link').removeClass('active');
        $(e.currentTarget).addClass('active');
        loadAllTransactions(1);
    });
});
