export const formatToImageUrl = (token: string) : string => {
    return `${process.env.REACT_APP_API_ENDPOINT}/images?token=${encodeURIComponent(token)}`
}