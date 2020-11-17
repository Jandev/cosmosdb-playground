//@ts-check
function helloworld() {
    var context = getContext();
    var response = context.getResponse();
    console.log('hello');
    response.setBody("Hello, World");
}

helloworld();

function getContext() {
    return {
        getResponse(){
            return { 
                setBody(){
                }
            }
        } 
    };
}