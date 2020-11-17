//@ts-check
function helloworld(items) {
    console.log('\n');
    console.log('--- Debugging starts here ---');
    console.log('\n');
    if (typeof items === "string") items = JSON.parse(items);
    items.forEach(function (a) {
        // do something here
        console.log(JSON.stringify(a));
        console.log('\n');
    });
    console.log(JSON.stringify(items));

    var collection = getContext().getCollection();

    console.log("--- Context ---")
    console.log('\n');
    console.log('\n');
    console.log(JSON.stringify(getContext()))
    console.log('\n');
    console.log('\n');
    console.log("--- Collection ---")
    console.log('\n');
    console.log(JSON.stringify(getContext().getCollection()))
    console.log('\n');
    console.log('\n');
    console.log("--- Selflink ---")
    console.log(JSON.stringify(collection.getSelfLink()))

    var collectionLink = collection.getSelfLink();
    var count = 0;

    if (!items) throw new Error("The input is undefined or null.");

    var numItems = items.length
    console.log("Found " + numItems + " in input.")
    console.log('\n');
    if (numItems == 0) {
        getContext().getResponse().setBody(0);
        return;
    }

    tryCreate(items[count], callback);

    function tryCreate(item, callback) {
        var options = { disableAutomaticIdGeneration: false };
        console.log("Trying to create " + JSON.stringify(item));
        console.log('\n');
        // Uncomment to show how transactions work
        // When an error is thrown in a stored procedure, everything will be rolled back.
        // if(item.Name == "Jan") {
        //     throw new Error("He's not allowed to travel!");
        // }
        var isAccepted = collection.createDocument(collectionLink, item, options, callback);
        if (!isAccepted) {
            getContext().getResponse().setBody(count);
        }
    }

    function callback(err, item, options) {
        if (err) throw err;

        count++;
        if (count >= numItems) {
            getContext().getResponse().setBody(count);
        } else {
            tryCreate(items[count], callback);
        }
    }
}