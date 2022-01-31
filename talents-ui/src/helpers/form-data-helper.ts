export const getFormData = (object: any) => {
    const formData = new FormData();
    Object.keys(object).forEach(key => {
        if (Array.isArray(object[key])) {
            for (var i = 0; i < object[key].length; i++) {
                Object.keys(object[key][i]).forEach(arrKey => {

                    var value = object[key][i][arrKey];

                    // if the value is a number but is set to undefined or null, change it to zero
                    if (!isNaN(value) && (value === undefined || value === null))
                        value = 0;

                    formData.append(`${key}[${i}].${arrKey}`, value);
                });
            }
        }
        else {
            var value = object[key];

            // if the value is a number but is set to undefined or null, change it to zero
            if (!isNaN(value) && (value === undefined || value === null))
                value = 0;
            formData.append(key, value);
        }
    });
    return formData;
}